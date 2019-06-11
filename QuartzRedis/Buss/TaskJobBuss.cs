using Newtonsoft.Json;
using QuartzRedis.Common;
using QuartzRedis.Dao;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static QuartzRedis.Dao.SqlDao;

namespace QuartzRedis.Buss
{
    public class TaskJobBuss
    {
        public async Task DoWork()
        {
            updateUserInfo();
            updateCommit();
            getCommit();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"DONE");
        }

        /// <summary>
        /// 上传用户信息 
        /// </summary>
        private void updateUserInfo()
        {
            try
            {
                SqlDao sqlDao = new SqlDao();
                UpdateUserInfoParam updateUserInfoParam = sqlDao.getAddMemberInfoParam();
                List<AddMemberInfoParam> paramList = updateUserInfoParam.AddMemberInfoParamList;
                Console.WriteLine("updateUserInfo:"+ paramList.Count);
                if (paramList.Count == 0)
                {
                    return;
                }
                List<String> list = new List<string>();
                foreach (var param in paramList)
                {
                    string st = getRemoteParam(param, "AddMemberInfo", "3");
                    string result = HttpHandle.PostHttps(Global.PostUrl, st, "application/json");
                    ReturnItem ri = JsonConvert.DeserializeObject<ReturnItem>(result);
                    if (ri.success)
                    {
                        List<string> list1 = updateUserInfoParam.Dictionary[param.phone];
                        foreach (var sql in list1)
                        {
                            list.Add(sql);
                        }
                    }
                    else
                    {
                        list.Add(sqlDao.postFailSql(param, ri.msg.msg));
                    }
                }
                DBHelp.ExecuteSqlTran(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 上传积分变动信息
        /// </summary>
        private void updateCommit()
        {
            try
            {
                SqlDao sqlDao = new SqlDao();
                List<String> list = new List<string>();
                //处理玩偶兑换积分
                List<GGiftSellMasterChangeParam> gList = sqlDao.getGGiftSellMasterChange();
                Console.WriteLine("updateCommit_GGiftSellMaster:" + gList.Count);
                foreach (GGiftSellMasterChangeParam gParam in gList)
                {
                    int point = 0;
                    try
                    {
                        point = Convert.ToInt32(gParam.GTM_Score);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(gParam.GTM_Score);
                    }
                    //GT_Type为6是增加积分，为3是减少积分
                    if (gParam.GT_Type == "3")
                    {
                        point = 0 - point;
                    }
                    AddPointRecordParam param = new AddPointRecordParam
                    {
                        phone = gParam.ME_MobileNum,
                        point = point.ToString(),
                    };
                    string st = getRemoteParam(param, "AddPointRecord", "3");
                    string result = HttpHandle.PostHttps(Global.PostUrl, st, "application/json");
                    ReturnItem ri = JsonConvert.DeserializeObject<ReturnItem>(result);
                    if (ri.success)
                    {
                        //玩偶兑换积分
                        StringBuilder builder = new StringBuilder();
                        builder.AppendFormat(ShipSqls.INSERT_GGIFTSELLMASTERLOG, gParam.GT_GUID, "1", point, "");
                        list.Add(builder.ToString());
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendFormat(ShipSqls.INSERT_GGIFTSELLMASTERLOG, gParam.GT_GUID, "0", point, "");
                        list.Add(builder.ToString());
                    }
                }
                //处理充值积分
                List<GRechargeDetailChangeParam> rList = sqlDao.getGRechargeDetailChange();
                Console.WriteLine("updateCommit_GRechargeDetail:" + rList.Count);
                foreach (GRechargeDetailChangeParam rParam in rList)
                {
                    int point = 0;
                    try
                    {
                        point = Convert.ToInt32(rParam.RD_GiveScore);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(rParam.RD_GiveScore);
                    }
                    //GT_Type为21和0是增加积分，为1和20是减少积分
                    if (rParam.RD_Type == "1" || rParam.RD_Type == "20")
                    {
                        point = 0 - point;
                    }
                    AddPointRecordParam param = new AddPointRecordParam
                    {
                        phone = rParam.ME_MobileNum,
                        point = point.ToString(),
                    };
                    string st = getRemoteParam(param, "AddPointRecord", "3");
                    string result = HttpHandle.PostHttps(Global.PostUrl, st, "application/json");
                    ReturnItem ri = JsonConvert.DeserializeObject<ReturnItem>(result);
                    if (ri.success)
                    {
                        StringBuilder builder1 = new StringBuilder();
                        builder1.AppendFormat(ShipSqls.INSERT_GRECHARGEDETAILLOG, rParam.RD_GUID, "1", point);
                        list.Add(builder1.ToString());
                    }
                    else
                    {
                        StringBuilder builder1 = new StringBuilder();
                        builder1.AppendFormat(ShipSqls.INSERT_GRECHARGEDETAILLOG, rParam.RD_GUID, "0", point);
                        list.Add(builder1.ToString());
                    }
                }
                //处理积分兑换币
                List<GChangeScoreDetailChangeParam> cList = sqlDao.GChangeScoreDetailChange();
                Console.WriteLine("updateCommit_GChangeScoreDetail:" + cList.Count);
                foreach (GChangeScoreDetailChangeParam cParam in cList)
                {
                    int point = 0;
                    try
                    {
                        point = 0 - Convert.ToInt32(cParam.CH_Money);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(cParam.CH_Money);
                    }
                    AddPointRecordParam param = new AddPointRecordParam
                    {
                        phone = cParam.ME_MobileNum,
                        point = point.ToString(),
                    };
                    string st = getRemoteParam(param, "AddPointRecord", "3");
                    string result = HttpHandle.PostHttps(Global.PostUrl, st, "application/json");
                    ReturnItem ri = JsonConvert.DeserializeObject<ReturnItem>(result);
                    if (ri.success)
                    {
                        StringBuilder builder1 = new StringBuilder();
                        builder1.AppendFormat(ShipSqls.INSERT_GCHANGESCOREDETAILLOG, cParam.CH_GUID, "1", point);
                        list.Add(builder1.ToString());
                    }
                    else
                    {
                        StringBuilder builder1 = new StringBuilder();
                        builder1.AppendFormat(ShipSqls.INSERT_GCHANGESCOREDETAILLOG, cParam.CH_GUID, "0", point);
                        list.Add(builder1.ToString());
                    }
                }

                DBHelp.ExecuteSqlTran(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        /// <summary>
        /// 获取积分变动信息
        /// </summary>
        private void getCommit()
        {
            try
            {
                SqlDao sqlDao = new SqlDao();
                string st = getRemoteParam(new Param(), "GetPointCommitList", "3");
                string result = HttpHandle.PostHttps(Global.PostUrl, st, "application/json");
                ReturnItem ri = JsonConvert.DeserializeObject<ReturnItem>(result);
                if (ri.success)
                {
                    if (ri.data != null)
                    {
                        Console.WriteLine("getCommit:" + ri.data.Count);
                        for (int i = 0; i < ri.data.Count; i++)
                        {
                            if (ri.data[i].storeId == "3")
                            {
                                List<GMemberParam> list = sqlDao.getGMemberParam(ri.data[i].phone);
                                if (list.Count > 0)
                                {
                                    int rPoint = 0;
                                    try
                                    {
                                        rPoint = Convert.ToInt16(ri.data[i].point);
                                    }
                                    catch (Exception)
                                    {
                                        sqlDao.insertRemoteLog(result);
                                    }
                                    List<string> idList = new List<string>();
                                    foreach (var param in list)
                                    {
                                        string id = DateTime.Now.ToString("yyyyMMdd") + "-" + param.ME_ShopNo + "-GXJGMM-" + DateTime.Now.ToString("HHmmss");
                                        int newPoint = 0, reducePoint = 0;
                                        try
                                        {
                                            newPoint = Convert.ToInt16(param.ME_Score);
                                        }
                                        catch (Exception)
                                        {
                                            sqlDao.insertRemoteLog(result);
                                            sqlDao.insertRemoteLog(param.ME_Score);
                                        }
                                        if (newPoint >= rPoint)
                                        {
                                            newPoint -= rPoint;
                                            reducePoint = rPoint;
                                            rPoint = 0;
                                        }
                                        else
                                        {
                                            newPoint = 0;
                                            reducePoint = newPoint;
                                            rPoint -= newPoint;
                                        }

                                        if (sqlDao.insertGGiftSellMaster(param, id, newPoint, reducePoint) > 0)
                                        {
                                            sqlDao.updateGMember(newPoint.ToString(), param.ME_ID);
                                            sqlDao.insertGGiftSellMasterLog(id, "0", ri.data[i].point, ri.data[i].pointCommitId);
                                            idList.Add(id);
                                        }
                                        if (rPoint == 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (rPoint == 0)
                                    {
                                        UpdatePointCommitParam param = new UpdatePointCommitParam
                                        {
                                            pointCommitId = ri.data[i].pointCommitId,
                                        };
                                        string st1 = getRemoteParam(param, "UpdatePointCommit", "3");
                                        string result1 = HttpHandle.PostHttps(Global.PostUrl, st1, "application/json");
                                        ReturnItem ri1 = JsonConvert.DeserializeObject<ReturnItem>(result);
                                        if (ri.success)
                                        {
                                            foreach (string id in idList)
                                            {
                                                sqlDao.updateGGiftSellMasterLog(id);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("UpdatePointCommit:" + result1);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }




        private string getRemoteParam(Param param, string name, string code)
        {
            string appId = Global.AppId;
            string appSecret = Global.AppSecret;
            //string code = "1";
            string placeHold = Global.PlaceHold;
            string nonceStr = DateTime.Now.ToString("MMddHHmmss");
            string paramS = Regex.Replace(JsonConvert.SerializeObject(param), "\"(.+?)\"",
                 new MatchEvaluator(
                    (s) =>
                    {
                        return s.ToString().Replace(" ", placeHold);
                    }))
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace(" ", "")
                    .Replace(placeHold, " ");
            string needMd5 = appId + nonceStr + appSecret + paramS;
            string md5S = "";
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(needMd5));
                var strResult = BitConverter.ToString(result);
                md5S = strResult.Replace("-", "");
            }

            PostParam postParam = new PostParam
            {
                sign = md5S,
                code = code,
                nonceStr = nonceStr,
                method = name,
                appId = appId,
                param = param,
            };

            return JsonConvert.SerializeObject(postParam);
        }


    }
}
