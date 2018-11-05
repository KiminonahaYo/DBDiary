using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DBClassLib.All.Base;
using DBClassLib.Common.Info;

namespace Diary.Forms
{
    /// <summary>
    ///     日記用フォーム
    /// </summary>
    public partial class FrmMain : Form
    {
        /// <summary>
        ///     ロガー
        /// </summary>
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private List<(DateTime? WriteDateTime, long? SeqNo, string Text)> lstLoad = new List<(DateTime? WriteDateTime, long? SeqNo, string Text)>();

        private (DateTime? WriteDateTime, long? SeqNo, string Text)? clsEditing = null;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
        }

        #region イベント

        #region フォームイベント
        /// <summary>
        ///     フォームロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            dtWriteDateTime_ValueChanged(this.dtWriteDateTime, EventArgs.Empty);
        }

        /// <summary>
        ///     フォーム表示時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///     フォームクローズ前処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        #endregion

        #region メニュー・ボタン
        /// <summary>
        ///     保存メニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSave_Click(object sender, EventArgs e)
        {
            if (this.clsEditing == null)
            {
                MessageBox.Show("日記を記述するか選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            bool blRet;

            if (this.clsEditing.Value.SeqNo == null)
            {
                blRet = this.DBInsert(this.clsEditing.Value.Text);
            }
            else
            {
                blRet = this.DBUpdate(this.clsEditing.Value.SeqNo.Value, this.clsEditing.Value.Text);
            }

            if (blRet)
            {
                MessageBox.Show("保存に成功しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("保存に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!this.DBLoad(this.dtWriteDateTime.Value.Date, ref this.lstLoad))
            {
                MessageBox.Show("日記の読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     破棄メニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuDestroy_Click(object sender, EventArgs e)
        {
            this.txtDiary.BackColor = Color.White;
            this.clsEditing = null;
            this.txtDiary.Text = "";
        }

        /// <summary>
        ///     閉じるメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///     保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.menuSave_Click(this.menuSave, EventArgs.Empty);
        }

        /// <summary>
        ///     破棄ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDestroy_Click(object sender, EventArgs e)
        {
            this.menuDestroy_Click(this.menuDestroy, EventArgs.Empty);
        }

        /// <summary>
        ///     削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.lstDiaryList.SelectedIndex < 0 || this.lstDiaryList.SelectedIndex >= this.lstLoad.Count) 
            {
                MessageBox.Show("削除する日記を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var cls = this.lstLoad[this.lstDiaryList.SelectedIndex];

            //確認メッセージを表示する。
            string strPreview = cls.Text.Length <= 20 ? cls.Text : cls.Text.Substring(0, 20) + "…";
            if (MessageBox.Show(cls.WriteDateTime.Value.ToString("yyyy年M月d日 HH:mm") + "の日記を削除しますか？\n\n内容：" + strPreview, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            //日記の削除を行う。
            if (this.DBDelete(cls.SeqNo.Value))
            {
                //成功
                MessageBox.Show("日記の削除に成功しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //失敗
                MessageBox.Show("日記の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //リロード
            if (!this.DBLoad(this.dtWriteDateTime.Value.Date, ref this.lstLoad))
            {
                MessageBox.Show("日記の読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.clsEditing = null;
            this.txtDiary.Text = "";
            this.txtDiary.BackColor = Color.White;
        }
        #endregion

        /// <summary>
        ///     リストボックスがダブルクリックされたら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstDiaryList_DoubleClick(object sender, EventArgs e)
        {
            if (this.lstDiaryList.SelectedIndex < 0 || this.lstDiaryList.SelectedIndex >= this.lstLoad.Count) return;

            var cls = this.lstLoad[this.lstDiaryList.SelectedIndex];

            this.clsEditing = cls;

            this.txtDiary.Text = cls.Text;

            this.txtDiary.BackColor = Color.LightCyan;
        }
        
        /// <summary>
        ///     DateTimePickerの日付が変更されたら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtWriteDateTime_ValueChanged(object sender, EventArgs e)
        {
            if (!this.DBLoad(this.dtWriteDateTime.Value.Date, ref this.lstLoad))
            {
                MessageBox.Show("日記の読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.clsEditing = null;
            this.txtDiary.BackColor = Color.White;
            this.txtDiary.Text = "";
        }

        /// <summary>
        ///     テキストが変更されたら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDiary_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            if (this.clsEditing == null || this.clsEditing.Value.SeqNo == null)
            {
                //未保存
                if (txt.Text.Length > 0)
                    this.clsEditing = (null, null, txt.Text);
                else
                    this.clsEditing = null;
            }
            else
            {
                //保存済み
                this.clsEditing = (this.clsEditing.Value.WriteDateTime, this.clsEditing.Value.SeqNo, txt.Text);
            }
        }

        #endregion

        #region イベントとは関係のない内部メソッド
        /// <summary>
        ///     データベースからデータをロードする。
        /// </summary>
        /// <param name="dtWriteDate"></param>
        /// <param name="lstResult"></param>
        /// <returns>true : アクセス成功, false アクセス失敗</returns>
        private bool DBLoad(DateTime dtWriteDate, ref List<(DateTime? WriteDateTime, long? SeqNo, string Text)> lstResult)
        {
            lstResult.Clear();

            using (DbConnection con = Common.CommonMethod.CreateConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }

                try
                {
                    DiaryDBLib.Classes.ClsDiary tableDiary = new DiaryDBLib.Classes.ClsDiary(con);

                    string strWhere = "Convert(Date, WRITE_DATETIME) = @WRITEDATETIME";

                    tableDiary.AddParameter("@WRITEDATETIME", dtWriteDate.Date, DBClassLib.Common.DBDataType.Date);
                    
                    List<OrderByInfo<DiaryDBLib.Classes.ClsDiary.Columns>> lstOrderBy = new List<OrderByInfo<DiaryDBLib.Classes.ClsDiary.Columns>>
                    {
                        new OrderByInfo<DiaryDBLib.Classes.ClsDiary.Columns>(DiaryDBLib.Classes.ClsDiary.Columns.WRITE_DATETIME, DBClassLib.Common.OrderByAscDesc.Asc)
                    };

                    using (DataSet ds = tableDiary.Select(null, strWhere, lstOrderBy))
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            lstResult.Add((
                                    tableDiary.ConvertDataRow<DateTime>(row["WRITE_DATETIME"]),
                                    tableDiary.ConvertDataRow<long>(row["SEQ_NO"]),
                                    tableDiary.ConvertDataRow<string>(row["VALUE"])
                                ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }

            this.lstDiaryList.Items.Clear();
            foreach (var cls in lstResult)
            {
                string strPreview = cls.Text.Length <= 10 ? cls.Text : cls.Text.Substring(0, 10) + "…";
                this.lstDiaryList.Items.Add(cls.WriteDateTime.Value.ToString("HH:mm - ") + strPreview);
            }

            return true;
        }

        /// <summary>
        ///     データベースに日記を登録する。
        /// </summary>
        /// <param name="strText">内容</param>
        /// <returns>true : アクセス成功, false アクセス失敗</returns>
        private bool DBInsert(string strText)
        {
            using (DbConnection con = Common.CommonMethod.CreateConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }

                try
                {
                    using (DbTransaction trans = new DbTransaction(con))
                    {
                        trans.BeginTrans();

                        try
                        {
                            DiaryDBLib.Classes.ClsDiary tableDiary = new DiaryDBLib.Classes.ClsDiary(trans);
                            Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object> diInsert = new Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object>();

                            long lngSeq = tableDiary.GetMaxSeqNo() + 1;

                            diInsert.Add(DiaryDBLib.Classes.ClsDiary.Columns.SEQ_NO, lngSeq);
                            diInsert.Add(DiaryDBLib.Classes.ClsDiary.Columns.WRITE_DATETIME, "SysDate");
                            diInsert.Add(DiaryDBLib.Classes.ClsDiary.Columns.VALUE, strText);
                            diInsert.Add(DiaryDBLib.Classes.ClsDiary.Columns.INS_TIME, "SysDate");

                            tableDiary.Insert(diInsert);

                            //成功時はコミット
                            trans.CommitTrans();

                            this.clsEditing = (null, lngSeq, strText);
                            this.txtDiary.BackColor = Color.LightCyan;
                        }
                        catch (Exception)
                        {
                            //エラー時はロールバック
                            trans.RollbackTrans();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }

            return true;
        }

        /// <summary>
        ///     データベースの日記を更新する。
        /// </summary>
        /// <param name="lngSeq">SEQ-NO</param>
        /// <param name="strText">内容</param>
        /// <returns>true : アクセス成功, false アクセス失敗</returns>
        private bool DBUpdate(long lngSeq, string strText)
        {
            using (DbConnection con = Common.CommonMethod.CreateConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }

                try
                {
                    using (DbTransaction trans = new DbTransaction(con))
                    {
                        trans.BeginTrans();

                        try
                        {
                            DiaryDBLib.Classes.ClsDiary tableDiary = new DiaryDBLib.Classes.ClsDiary(trans);
                            Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object> diUpdate = new Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object>();

                            diUpdate.Add(DiaryDBLib.Classes.ClsDiary.Columns.SEQ_NO, lngSeq);
                            diUpdate.Add(DiaryDBLib.Classes.ClsDiary.Columns.VALUE, strText);
                            diUpdate.Add(DiaryDBLib.Classes.ClsDiary.Columns.UPD_TIME, "SysDate");

                            tableDiary.Update(diUpdate);

                            //成功時はコミット
                            trans.CommitTrans();
                        }
                        catch (Exception)
                        {
                            //エラー時はロールバック
                            trans.RollbackTrans();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }

            return true;
        }

        /// <summary>
        ///     データベースの日記を削除する。
        /// </summary>
        /// <param name="lngSeq">SEQ-NO</param>
        /// <returns>true : アクセス成功, false アクセス失敗</returns>
        private bool DBDelete(long lngSeq)
        {
            using (DbConnection con = Common.CommonMethod.CreateConnection())
            {
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }

                try
                {
                    using (DbTransaction trans = new DbTransaction(con))
                    {
                        trans.BeginTrans();

                        try
                        {
                            DiaryDBLib.Classes.ClsDiary clsDiary = new DiaryDBLib.Classes.ClsDiary(trans);
                            Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object> diWhere = new Dictionary<DiaryDBLib.Classes.ClsDiary.Columns, object>();

                            diWhere.Add(DiaryDBLib.Classes.ClsDiary.Columns.SEQ_NO, lngSeq);

                            clsDiary.Delete(diWhere);

                            //成功時はコミット
                            trans.CommitTrans();
                        }
                        catch (Exception)
                        {
                            //エラー時はロールバック
                            trans.RollbackTrans();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }

            return true;
        }
        #endregion

    }
}
