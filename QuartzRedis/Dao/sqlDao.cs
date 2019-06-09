using QuartzRedis.Buss;
using QuartzRedis.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QuartzRedis.Dao
{
    class SqlDao
    {
        public List<AddMemberInfoParam> getAddMemberInfoParam()
        {
            List<AddMemberInfoParam> list = new List<AddMemberInfoParam>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GMEMBER);
            string sql = builder.ToString();
            DataTable dt = DBHelp.Query(sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AddMemberInfoParam param = new AddMemberInfoParam
                    {
                        phone = dr["ME_MobileNum"].ToString(),
                        point = dr["ME_Score"].ToString(),
                        cardCode = dr["ME_ID"].ToString(),
                    };
                    list.Add(param);
                }
            }
            return list;
        }
        public void postSuccessSql(ref List<string> list, AddMemberInfoParam param)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.INSERT_GMEMBERLOG, param.cardCode, param.phone,"1", param.point);
            list.Add(builder.ToString());
            builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GRECHARGEDETAIL_BY_ME_MOBILENUM, param.phone);
            string sql = builder.ToString();
            DataTable dt = DBHelp.Query(sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder builder1 = new StringBuilder();
                    builder1.AppendFormat(ShipSqls.INSERT_GRECHARGEDETAILLOG, dr["RD_GUID"].ToString(), "1", "同步会员");
                    list.Add(builder1.ToString());
                }
            }
            builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GGIFTSELLMASTER_BY_ME_MOBILENUM, param.phone);
            string sql1 = builder.ToString();
            DataTable dt1 = DBHelp.Query(sql1).Tables[0];
            if (dt1 != null)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    StringBuilder builder1 = new StringBuilder();
                    builder1.AppendFormat(ShipSqls.INSERT_GGIFTSELLMASTERLOG, dr["GT_GUID"].ToString(), "1", "同步会员", "");
                    list.Add(builder1.ToString());
                }
            }
        }
        public string postFailSql(AddMemberInfoParam param,string msg)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.INSERT_GMEMBERLOG, param.cardCode, param.phone,"0", msg);
            return builder.ToString();
        }


        public List<GGiftSellMasterChangeParam> getGGiftSellMasterChange()
        {
            List<GGiftSellMasterChangeParam> list = new List<GGiftSellMasterChangeParam>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GGIFTSELLMASTER_CHANGE);
            string sql = builder.ToString();
            DataTable dt = DBHelp.Query(sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GGiftSellMasterChangeParam param = new GGiftSellMasterChangeParam
                    {
                        GT_GUID = dr["GT_GUID"].ToString(),
                        ME_MobileNum = dr["ME_MOBILENUM"].ToString(),
                        GTM_Score = dr["GTM_SCORE"].ToString(),
                        GT_Type = dr["GT_TYPE"].ToString(),
                    };
                    list.Add(param);
                }
            }
            return list;
        }

        public List<GRechargeDetailChangeParam> getGRechargeDetailChange()
        {
            List<GRechargeDetailChangeParam> list = new List<GRechargeDetailChangeParam>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GRECHARGEDETAIL_CHANGE);
            string sql = builder.ToString();
            DataTable dt = DBHelp.Query(sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GRechargeDetailChangeParam param = new GRechargeDetailChangeParam
                    {
                        RD_GUID = dr["RD_GUID"].ToString(),
                        ME_MobileNum = dr["ME_MOBILENUM"].ToString(),
                        RD_GiveScore = dr["RD_GIVESCORE"].ToString(),
                        RD_Type = dr["RD_TYPE"].ToString(),
                    };
                    list.Add(param);
                }
            }
            return list;
        }

        public List<GMemberParam> getGMemberParam(string phone)
        {
            List<GMemberParam> list = new List<GMemberParam>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.SELECT_GMEMBER_BY_ME_MOBILENUM,phone);
            string sql = builder.ToString();
            DataTable dt = DBHelp.Query(sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GMemberParam param = new GMemberParam
                    {
                        ME_ID = dr["ME_ID"].ToString(),
                        ME_Name = dr["ME_Name"].ToString(),
                        ME_Point = dr["ME_Point"].ToString(),
                        ME_Score = dr["ME_Score"].ToString(),
                        ME_Coin = dr["ME_Coin"].ToString(),
                        ME_MobileNum = dr["ME_MobileNum"].ToString(),
                        ME_Ticket = dr["ME_Ticket"].ToString(),
                        ME_ShopNo = dr["ME_ShopNo"].ToString(),
                        ME_F_CT_ID = dr["ME_F_CT_ID"].ToString(),
                    };
                    list.Add(param);
                }
            }
            return list;
        }

        public int insertRemoteLog(string errValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.INSERT_REMOTELOG,errValue);
            string sql = builder.ToString();
            return DBHelp.ExecuteSql(sql);
        }

        public int insertGGiftSellMaster(GMemberParam param,string id,int newPoint,int reducePoint)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.INSERT_GGIFTSELLMASTER, id, param.ME_F_CT_ID, param.ME_ShopNo + "-ADMIN", param.ME_ID,
                param.ME_Name, reducePoint, reducePoint, param.ME_Point, param.ME_Coin, param.ME_Ticket,newPoint, param.ME_ShopNo);
            string sql = builder.ToString();
            return DBHelp.ExecuteSql(sql);
        }
        public int updateGMember(string ME_SCORE,string ME_ID)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.UPDATE_GMEMBER, ME_SCORE, ME_ID);
            string sql = builder.ToString();
            return DBHelp.ExecuteSql(sql);
        }

        public int insertGGiftSellMasterLog(string GT_GUID, string IFGIFT, string GIFT_REMARK,string POINTCOMMITID)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.INSERT_GGIFTSELLMASTERLOG, GT_GUID, IFGIFT, GIFT_REMARK, POINTCOMMITID);
            string sql = builder.ToString();
            return DBHelp.ExecuteSql(sql);
        }

        public int updateGGiftSellMasterLog(string GT_GUID)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(ShipSqls.UPDATE_GGIFTSELLMASTERLOG, GT_GUID);
            string sql = builder.ToString();
            return DBHelp.ExecuteSql(sql);
        }

        public class ShipSqls
        {
            public const string SELECT_GMEMBER = "" +
                                      "SELECT TOP(1000) MAX(ME_ID) AS ME_ID,ME_MOBILENUM,SUM(ME_SCORE) ME_SCORE FROM GMEMBER " +
                                      "WHERE ME_MOBILENUM NOT IN (SELECT ME_MOBILENUM FROM GMEMBERLOG) " +
                                      "AND LEN(ME_MOBILENUM)=11 AND ME_MOBILENUM LIKE '1%' " +
                                      "GROUP BY ME_MOBILENUM ";
            public const string INSERT_GMEMBERLOG = "" +
                                      "INSERT INTO GMEMBERLOG(ME_ID,ME_MOBILENUM,IFGIFT,GIFT_REMARK) " +
                                      "VALUES('{0}','{1}','{2}','{3}')";
            public const string SELECT_GRECHARGEDETAIL_BY_ME_MOBILENUM = "" +
                                      "SELECT RD_GUID FROM GRECHARGEDETAIL " +
                                      "WHERE RD_ME_ID IN (SELECT ME_ID FROM GMEMBER WHERE ME_MOBILENUM = '{0}')";
            public const string INSERT_GRECHARGEDETAILLOG = "" +
                                      "INSERT INTO GRECHARGEDETAILLOG(RD_GUID,IFGIFT,GIFT_REMARK) " +
                                      "VALUES('{0}','{1}','{2}')";
            public const string SELECT_GGIFTSELLMASTER_BY_ME_MOBILENUM = "" +
                                      "SELECT GT_GUID FROM GGIFTSELLMASTER " +
                                      "WHERE GTM_ME_ID IN (SELECT ME_ID FROM GMEMBER WHERE ME_MOBILENUM = '{0}')";
            public const string INSERT_GGIFTSELLMASTERLOG = "" +
                                      "INSERT INTO GGIFTSELLMASTERLOG(GT_GUID,IFGIFT,GIFT_REMARK,POINTCOMMITID) " +
                                      "VALUES('{0}','{1}','{2}','{3}')";

            public const string SELECT_GGIFTSELLMASTER_CHANGE = "" +
                                      "SELECT GS.GT_GUID,GM.ME_MOBILENUM,GS.GTM_SCORE,GS.GT_TYPE " +
                                      "FROM GGIFTSELLMASTER GS,GMEMBERLOG GM " +
                                      "WHERE GS.GTM_TIME > CONVERT(DATETIME,'06/09/2019',101) AND GS.GTM_ME_ID = GM.ME_ID  " +
                                      "AND GS.GT_GUID NOT IN (SELECT GT_GUID FROM GGIFTSELLMASTERLOG) AND GS.GTM_SCORE >0 " +
                                      "ORDER BY GS.GTM_TIME ASC";
            public const string SELECT_GRECHARGEDETAIL_CHANGE = "" +
                                      "SELECT  GR.RD_GUID,GM.ME_MOBILENUM,GR.RD_GIVESCORE,GR.RD_TYPE " +
                                      "FROM GRECHARGEDETAIL GR,GMEMBERLOG GM " +
                                      "WHERE GR.RD_TIME > CONVERT(DATETIME,'06/09/2019',101) AND GR.RD_ME_ID = GM.ME_ID " +
                                      "AND GR.RD_GUID NOT IN (SELECT RD_GUID FROM GRECHARGEDETAILLOG) AND GR.RD_GIVESCORE > 0 " +
                                      "ORDER BY GR.RD_TIME ASC";

            public const string SELECT_GMEMBER_BY_ME_MOBILENUM = "" +
                                      "SELECT GM.ME_ID,GM.ME_NAME,GM.ME_POINT,GM.ME_SCORE,GM.ME_COIN,GM.ME_TICKET,GM.ME_SHOPNO, " +
                                             "GL.ME_MOBILENUM,GM.ME_F_CT_ID " +
                                      "FROM GMEMBER GM,GMEMBERLOG GL " +
                                      "WHERE GM.ME_MOBILENUM = GL.ME_MOBILENUM AND GM.ME_SCORE>0 AND GL.ME_MOBILENUM = '{0}' " +
                                      "ORDER BY GL.ME_MOBILENUM, ME_SCORE DESC";
            public const string INSERT_REMOTELOG = "" +
                                      "INSERT INTO REMOTELOG(INPUTTIME,ERRVALUE) VALUES(GETDATE(),'{0}')";

            public const string INSERT_GGIFTSELLMASTER = "" +
                                      "INSERT INTO GGIFTSELLMASTER(GT_GUID,GTM_TIME,GTM_CT_ID,GTM_QP_ID," +
                                        "GTM_ME_ID,GT_ME_NAME,GTM_NUM,GTM_MONEY," +
                                        "GTM_POINT,GTM_COIN,GTM_TICKET,GTM_SCORE," +
                                        "GTM_MAC,GTM_ME_POINT,GTM_ME_COIN,GTM_ME_TICKET," +
                                        "GTM_ME_SCORE,GT_TYPE,GT_SHOPNO)" +
                                        " VALUES('{0}',GETDATE(),'{1}','{2}'," +
                                        "'{3}','{4}',1,{5}," +
                                        "0,0,0,{6}," +
                                        "100,{7},{8},{9},{10},3,'{11}')";

            public const string UPDATE_GMEMBER = "" +
                                      "UPDATE GMEMBER SET ME_SCORE = {0} WHERE ME_ID='{1}'";
            public const string UPDATE_GGIFTSELLMASTERLOG = "" +
                                      "UPDATE GGIFTSELLMASTERLOG SET IFGIFT=1 WHERE GT_GUID ='{0}'";
        }
    }
}
