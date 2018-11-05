using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DBClassLib.All.Base;
using DBClassLib.All.Middle;
using DBClassLib.Common;
using DBClassLib.Common.Info;

namespace DiaryDBLib.Classes
{
    /// <summary>
    ///     日記のデータベースのアクセスクラス
    /// </summary>
    public class ClsDiary : DbTableBaseMiddle<ClsDiary.Columns>
    {
        /// <summary>
        ///     テーブル名
        /// </summary>
        public const string TABLE_NAME = "DAT_DIARY";

        /// <summary>
        ///     カラム名
        /// </summary>
        public enum Columns
        {
            /// <summary>連番</summary>
            SEQ_NO,
            /// <summary>記述日</summary>
            WRITE_DATETIME,
            /// <summary>日記の内容</summary>
            VALUE,
            /// <summary>登録日時</summary>
            INS_TIME,
            /// <summary>更新日時</summary>
            UPD_TIME
        }

        #region コンストラクタ
        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="con">コネクション</param>
        public ClsDiary(DbConnection con) : base(con)
        {
        }

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="trans">トランザクション</param>
        public ClsDiary(DbTransaction trans) : base(trans)
        {
        }

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="con">コネクション</param>
        /// <param name="strSchema">スキーマ名</param>
        public ClsDiary(DbConnection con, string strSchema) : base(con, strSchema)
        {
        }

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="trans">トランザクション</param>
        /// <param name="strSchema">スキーマ名</param>
        public ClsDiary(DbTransaction trans, string strSchema) : base(trans, strSchema)
        {
        }
        #endregion

        /// <summary>
        ///     テーブル情報を取得する。
        /// </summary>
        /// <returns>テーブル情報</returns>
        public override TableInfo GetTableInfo()
        {
            TableInfo table = new TableInfo() { TableName = TABLE_NAME, Schema = this.Schema };

            table.Columns.Add(Columns.SEQ_NO.ToString(), DBDataType.Normal, true, false);
            table.Columns.Add(Columns.WRITE_DATETIME.ToString(), DBDataType.Date, false, false);
            table.Columns.Add(Columns.VALUE.ToString(), DBDataType.Normal, false, true);
            table.Columns.Add(Columns.INS_TIME.ToString(), DBDataType.Date, false, false);
            table.Columns.Add(Columns.UPD_TIME.ToString(), DBDataType.Date, false, true);

            return table;
        }

        /// <summary>
        ///     最大SEQ-NOを取得する。
        /// </summary>
        /// <returns>最大SEQ-NO</returns>
        public long GetMaxSeqNo()
        {
            string strRet = this.SelectFunc(Functions.Max, Columns.SEQ_NO);

            if (strRet == null) return 0;

            return long.Parse(strRet);
        }
    }
}
