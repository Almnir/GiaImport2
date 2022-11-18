using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraWizard;
using GiaImport2.Enumerations;
using GiaImport2.Models;
using GiaImport2.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class ExportInterviewWizard : DevExpress.XtraEditors.XtraForm
    {
        IInterviewRepository InterviewRepository;
        ICommonRepository CommonRepository;

        string ExportPath;
        IEnumerable<string> ExamDates;
        string CurrentExamDate;
        public List<Governmentinfo> ParticipantsExams;
        BindingList<SchoolClasses> SchoolClasses;
        SchoolsPanel SchoolsPanelControl;
        SchoolsClassesPanel SchoolsClassesPanelControl;
        TreeList TreeList;
        int SchoolClassesCount;

        public GridControl GetSchoolControl() => SchoolsPanelControl.GetGridControl();
        public GridView GetSchoolControlView() => SchoolsPanelControl.GetGridView();
        public TreeList GetTreeList() => SchoolsClassesPanelControl.GetTreeList();
        public int GetSchoolClassesCount() { return SchoolClassesCount; }

        public string ExportPathFolder { get; set; }
        public string ExamDate { get; set; }

        public List<(int School, string Class)> SchoolClassReturn { get; set; }

        public ChooseSchoolsOrClass SchoolsOrClassChoice;

        public ExportInterviewWizard(IInterviewRepository interviewRepository, ICommonRepository commonRepository)
        {
            InitializeComponent();
            this.InterviewRepository = interviewRepository;
            this.ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.ExamDates = null;
            this.SchoolClasses = null;
            SchoolsPanelControl = new SchoolsPanel();
            SchoolsClassesPanelControl = new SchoolsClassesPanel();
            CommonRepository = commonRepository;
            SchoolClassReturn = new List<(int, string)>();
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            if (splashScreenManager1.IsSplashFormVisible)
                splashScreenManager1.CloseWaitForm();
            this.Close();
        }

        private async void wizardControl1_SelectedPageChanged(object sender, DevExpress.XtraWizard.WizardPageChangedEventArgs e)
        {
            // если выбрана первая страница
            if (e.Page == welcomeWizardPage1)
            {
                if (this.ExamDates == null)
                {
                    splashScreenManager1.ShowWaitForm();
                    this.ExamDates = await InterviewRepository.GetExamDates();
                    if (this.ExamDates != null)
                    {
                        ComboBoxItemCollection itemsCollection = ExamDatesCombo.Properties.Items;
                        itemsCollection.BeginUpdate();
                        try
                        {
                            foreach (var item in this.ExamDates)
                            {
                                itemsCollection.Add(item);
                            }    
                        }
                        finally
                        {
                            itemsCollection.EndUpdate();
                        }
                        // выбираем первый пункт
                        ExamDatesCombo.SelectedIndex = 0;
                    }
                    splashScreenManager1.CloseWaitForm();
                }
                ExportPathEdit.Text = this.ExportPath;
            }
            if (e.Page == processWizardPage)
            {
                splashScreenManager1.ShowWaitForm();

                var examDate = (string)this.ExamDatesCombo.SelectedItem;
                if (examDate == null || (examDate != null && CurrentExamDate != examDate))
                {
                    ParticipantsExams = await InterviewRepository.GetParticipantsExamsData(examDate);
                }
                if (ParticipantsExams.Count == 0)
                {
                    splashScreenManager1.CloseWaitForm();
                    FormsHelper.ShowStyledMessageBox("Внимание!", "Нет данных распределения на выбранную дату!");
                    e.Page.AllowNext = false;
                    wizardControl1.SelectedPage = welcomeWizardPage1;
                    return;
                }
                else
                {
                    e.Page.AllowNext = true;
                }
                // проверка на доступ к подкаталогу
                if (!CommonRepository.CheckAccessToFolder(this.ExportPathEdit.Text.Trim()))
                {
                    splashScreenManager1.CloseWaitForm();
                    FormsHelper.ShowStyledMessageBox("Внимание!", "Нет доступа к выбранному подкаталогу!");
                    e.Page.AllowNext = false;
                    wizardControl1.SelectedPage = welcomeWizardPage1;
                    return;
                }
                else
                {
                    e.Page.AllowNext = true;
                    this.ExportPathFolder = this.ExportPathEdit.Text;
                }
                ExamDate = ExamDatesCombo.EditValue.ToString();
                if (PartitionGroup.Properties.Items[PartitionGroup.SelectedIndex].Description == "По школам")
                {
                    // текущий выбор - школы
                    SchoolsOrClassChoice = ChooseSchoolsOrClass.Schools;
                    if (this.SchoolClasses == null || 
                        (this.SchoolClasses != null && 
                         this.SchoolClasses.Count != 0 && 
                         this.SchoolClasses[0].PClassName == ChooseSchoolsOrClass.Classes.Name) ||
                         CurrentExamDate != examDate)
                    {
                        // обновляем закэшированную дату
                        CurrentExamDate = examDate;
                        SchoolClasses = new BindingList<SchoolClasses>();
                        this.SchoolClasses = new BindingList<SchoolClasses>(ParticipantsExams.SelectMany(s => s.Schools).OrderBy(x => x.SchoolCode).Distinct()
                            .Select(x => new SchoolClasses(x.SchoolCode, x.ShortName, SchoolsOrClassChoice.ToString())).ToList());
                    }
                    processWizardPage.Controls.Clear();
                    SchoolsPanelControl.GetGridControl().DataSource = this.SchoolClasses;
                    SchoolsPanelControl.GetGridView().BestFitColumns();
                    SchoolsPanelControl.GetGridView().RefreshData();
                    SchoolsPanelControl.Dock = DockStyle.Fill;
                    processWizardPage.Controls.Add(SchoolsPanelControl);
                    processWizardPage.PerformLayout();
                }
                if (PartitionGroup.Properties.Items[PartitionGroup.SelectedIndex].Description == "По классам")
                {
                    // текущий выбор - классы
                    SchoolsOrClassChoice = ChooseSchoolsOrClass.Classes;
                    if (this.SchoolClasses == null || 
                        (this.SchoolClasses != null &&
                        this.SchoolClasses.Count != 0 &&
                        this.SchoolClasses[0].PClassName == ChooseSchoolsOrClass.Schools.ToString()) |
                        CurrentExamDate != examDate)
                    {
                        // обновляем закэшированную дату
                        CurrentExamDate = examDate;
                        SchoolClasses = new BindingList<SchoolClasses>();
                        int index = 0;
                        int parentID = 0;
                        // тут мы проходимся по всей выборке и формируем некое дерево для отображения в TreeList
                        foreach (var gs in ParticipantsExams.SelectMany(s => s.Schools).OrderBy(x => x.SchoolCode))
                        {
                            // элемент "школа" со сквозным индексом и индексом иерархии
                            // в данном случае это родительский элемент в отношении "одна школа" - "много классов"
                            // поэтому он(элемент) имеет индекс -1. Don't ask me why.
                            SchoolClasses schoolClasses = new SchoolClasses(index, -1, gs.SchoolCode, gs.ShortName, SchoolsOrClassChoice.ToString());
                            parentID = index;
                            SchoolClasses.Add(schoolClasses);
                            index++;
                            foreach (var ps in gs.Participants.GroupBy(x => x.PClass).OrderBy(o => o.Key))
                            {
                                // элемент "класс", он имеет иерархический индекс своей родительской школы
                                schoolClasses = new SchoolClasses(index, parentID, gs.SchoolCode, ps.Key, ps.Key);
                                SchoolClasses.Add(schoolClasses);
                                index++;
                            }
                        }
                    }
                    TreeList = this.SchoolsClassesPanelControl.GetTreeList();
                    TreeList.DataSource = this.SchoolClasses;
                    TreeList.ExpandAll();
                    // это мы долго искали чтобы оно сдвигало фокус на первый по сортировке данных элемента
                    if (TreeList.GetNodeList().Count != 0)
                        TreeList.SetFocusedNode(TreeList.GetNodeList()[0]);
                    // очищаем существующие контролы
                    processWizardPage.Controls.Clear();
                    this.SchoolsClassesPanelControl.Dock = DockStyle.Fill;
                    processWizardPage.Controls.Add(SchoolsClassesPanelControl);
                    processWizardPage.PerformLayout();
                }
                splashScreenManager1.CloseWaitForm();
            }
            if (e.Page == completionWizardPage1)
            {
                // очищаем вывод
                SchoolClassReturn.Clear();
                this.SchoolClassesCount = 0;
                List<string> info = new List<string>();
                // если это выбор дерева школоклассов и есть выбранные элементы
                if (TreeList != null && TreeList.GetAllCheckedNodes().Count != 0 && SchoolsOrClassChoice == ChooseSchoolsOrClass.Classes)
                {
                    foreach (TreeListNode treeListNode in TreeList.GetAllCheckedNodes())
                    {
                        if (treeListNode.ParentNode != null)
                        {
                            // формируем строки "школа - класс"
                            var ttext = $"{treeListNode.ParentNode.GetDisplayText(0)} - {treeListNode.GetDisplayText(0)}";
                            info.Add(ttext);

                            // набиваем возврат школа-класс
                            SchoolClassReturn.Add((((SchoolClasses)TreeList.GetDataRecordByNode(treeListNode.ParentNode)).SchoolCode, 
                                treeListNode.GetDisplayText(0)));
                        }
                    }
                }
                // если это выбор грида с школами и есть школы
                else if (SchoolsPanelControl.GetGridView().DataRowCount > 0 && SchoolsOrClassChoice == ChooseSchoolsOrClass.Schools)
                {
                    for (int i = 0; i < SchoolsPanelControl.GetGridView().DataRowCount; i++)
                    {
                        // формируем строку из короткого названия школы
                        if (SchoolsPanelControl.GetGridView().IsRowSelected(i))
                        {
                            info.Add((string)SchoolsPanelControl.GetGridView().GetRowCellValue(i, SchoolsPanelControl.GetGridView().Columns[1]));
                            this.SchoolClassesCount += 1;
                            // набиваем возврат
                            SchoolClassReturn.Add(((int)SchoolsPanelControl.GetGridView().GetRowCellValue(i, SchoolsPanelControl.GetGridView().Columns[0]), null));
                        }
                    }
                }
                if (info.Count == 0)
                {
                    e.Page.AllowFinish = false;
                    info.Add("Нет выбранных элементов");
                } else
                    e.Page.AllowFinish = true;
                BindingList<string> infoList = new BindingList<string>(info.OrderBy(x => x).ToList());
                InfoGrid.DataSource = info;
                InfoGrid.Refresh();
            }
        }

        private void ExpandDetails(GridView view)
        {
            GridViewInfo info = (GridViewInfo)view.GetViewInfo();
            foreach (GridDataRowInfo rInfo in info.RowsInfo)
            {
                view.ExpandMasterRow(rInfo.RowHandle);
                view.SetMasterRowExpanded(rInfo.RowHandle, true);
            }
        }

        private void ExportFolderButton_Click(object sender, EventArgs e)
        {
            XtraFolderBrowserDialog openDialog = new XtraFolderBrowserDialog();

            DialogResult userClicked = openDialog.ShowDialog();

            if (userClicked == DialogResult.Cancel)
            {
                return;
            }
            // проверяем на длину пути
            if (CheckIfPathTooLong(openDialog))
            {
                FormsHelper.ShowStyledMessageBox("Слишком длинный путь!", "Внимание!");
                return;
            }
            ExportPath = openDialog.SelectedPath;
            if (string.IsNullOrEmpty(ExportPath))
            {
                return;
            }
            ExportPathEdit.Text = ExportPath;
        }

        private bool CheckIfPathTooLong(XtraFolderBrowserDialog openDialog)
        {
            bool error = false;
            try
            {
                var pathnottoolong = openDialog.SelectedPath;
                int pathlen = Path.GetFullPath(pathnottoolong).Length;
                if (pathlen >= Globals.MAX_PATH)
                {
                    throw new PathTooLongException();
                }
            }
            catch (PathTooLongException)
            {
                error = true;
            }
            return error;
        }
    }
}