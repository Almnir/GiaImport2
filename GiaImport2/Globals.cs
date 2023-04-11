using System.Collections.Generic;

namespace GiaImport2
{
    public static class Globals
    {
        // максимальная длина будущего пути примерно 100, вроде бы корня в 100 должно хватить до 248 максимальных
        public const int MAX_PATH = 100;

        public static string ROOT_ELEMENT = "ns1:GIADBSet";

        // если не задано, то юзер каталог
        //public static string TEMP_DIR = string.IsNullOrWhiteSpace(frmSettings.TempDirectoryText) ? Path.GetTempPath() + @"\Tempdir\" : frmSettings.TempDirectoryText + @"\Tempdir\";

        public static string TARGET_CREDENTIAL = "GiaImport";

        public static FormSettings frmSettings = new FormSettings();

        public static string GetConnectionString(string serverText, string databaseText, string loginText, string passwordText)
        {
            return string.Format("Server={0};Database={1};User Id={2};Password={3};Application Name=GiaImport v.{4}", serverText, databaseText, loginText, passwordText, Properties.Settings.Default.Version);
        }

        public const string GRID_NAME = "Таблица";
        public const string GRID_DESCRIPTION = "Описание";
        public const string GRID_XML = "Записей в XML";
        public const string GRID_LOADER = "Записей в таблицах загрузки";
        public const string GRID_TOTAL = "Записей всего в БД";
        public const string GRID_NOT_LOADED = "Незагруженные записи";

        public const int PARTICIPANTS_IN_PAGE = 20;

        public const int TASKTYPECODE_CHECK = 0;
        public const int TASKTYPECODE_FINAL = 1;

        public const string DB_NAME_STARTS_WITH = "erbd_gia_reg_23_";
        public const string DB_VERSION_STARTS_WITH = "13.";

        public const string BARCODE_STARTS = "22320";

        public const string TEST_TYPE = "9";

        public static List<string> TABLES_NAMES = new List<string>()
        {
            "ac_AppealDecisions",
            "ac_AppealExperts",
            "ac_Appeals",
            "ac_AppealTasks",
            "ac_Changes",
            "ac_Corrections",
            "dats_Borders",
            "dats_Groups",
            "prnf_CertificatePrintMain",
            "rbd_Address",
            "rbd_Areas",
            "rbd_Auditoriums",
            "rbd_AuditoriumsSubjects",
            "rbd_CurrentRegion",
            "rbd_CurrentRegionAddress",
            "rbd_Experts",
            "rbd_ExpertsExams",
            "rbd_ExpertsSubjects",
            "rbd_Governments",
            "rbd_ParticipantClasses",
            "rbd_ParticipantProperties",
            "rbd_Participants",
            "rbd_ParticipantsExamPStation",
            "rbd_ParticipantsExams",
            "rbd_ParticipantsExamsOnSchool",
            "rbd_ParticipantsExamsOnStation",
            "rbd_ParticipantsProfSubject",
            "rbd_ParticipantsSubject",
            "rbd_Places",
            "rbd_RegionFiles",
            "rbd_SchoolAddress",
            "rbd_SchoolClasses",
            "rbd_SchoolParticipant",
            "rbd_Schools",
            "rbd_StationExamAuditory",
            "rbd_StationExamAuditoryProps",
            "rbd_StationForm",
            "rbd_StationFormAct",
            "rbd_StationFormAuditoryFields",
            "rbd_StationFormFields",
            "rbd_Stations",
            "rbd_StationsExams",
            "rbd_StationWorkerOnExam",
            "rbd_StationWorkerOnStation",
            "rbd_StationWorkers",
            "rbd_StationWorkersAccreditation",
            "rbd_StationWorkersSubjects",
            "rbd_StationWorkersPositions",
            "res_Answers",
            "res_Complects",
            "res_HumanTests",
            "res_Marks",
            "res_SubComplects",
            "res_Subtests",
            "sht_Alts",
            "sht_ExamFinish",
            "sht_FinalMarks_C",
            "sht_FinalMarks_D",
            "sht_Marks_AB",
            "sht_Marks_C",
            "sht_Marks_D",
            "sht_Packages",
            "sht_Sheets_AB",
            "sht_Sheets_C",
            "sht_Sheets_D",
            "sht_Sheets_R",
            "rbd_AuditoriumParallels",
            "rbd_ExpertParallels",
            "rbd_StationParallels",
            "rbd_StationWorkerParallels"
        };

        public static List<string> TABLES_NAMES_FOR_DELETE = new List<string>()
        {
            "ac_AppealDecisions",
            "ac_AppealExperts",
            "ac_Appeals",
            "ac_AppealTasks",
            "ac_Changes",
            "ac_Corrections",
            "dats_Borders",
            "dats_Groups",
            "prnf_CertificatePrintMain",
            "rbd_Address",
            "rbd_Areas",
            "rbd_Auditoriums",
            "rbd_AuditoriumsSubjects",
            "rbd_CurrentRegion",
            "rbd_CurrentRegionAddress",
            "rbd_Experts",
            "rbd_ExpertsExams",
            "rbd_ExpertsSubjects",
            "rbd_Governments",
            "rbd_ParticipantClasses",
            "rbd_ParticipantProperties",
            "rbd_Participants",
            "rbd_ParticipantsExamPStation",
            "rbd_ParticipantsExams",
            "rbd_ParticipantsExamsOnSchool",
            "rbd_ParticipantsExamsOnStation",
            "rbd_ParticipantsProfSubject",
            "rbd_ParticipantsSubject",
            "rbd_Places",
            "rbd_RegionFiles",
            "rbd_SchoolAddress",
            "rbd_SchoolClasses",
            "rbd_SchoolParticipant",
            "rbd_Schools",
            "rbd_StationExamAuditory",
            "rbd_StationExamAuditoryProps",
            "rbd_StationForm",
            "rbd_StationFormAct",
            "rbd_StationFormAuditoryFields",
            "rbd_StationFormFields",
            "rbd_Stations",
            "rbd_StationsExams",
            "rbd_StationWorkerOnExam",
            "rbd_StationWorkerOnStation",
            "rbd_StationWorkers",
            "rbd_StationWorkersAccreditation",
            "rbd_StationWorkersSubjects",
            "rbd_StationWorkersPositions",
            "res_Answers",
            "res_Complects",
            "res_HumanTests",
            "res_Marks",
            "res_SubComplects",
            "res_Subtests",
            "sht_Alts",
            "sht_ExamFinish",
            "sht_FinalMarks_C",
            "sht_FinalMarks_D",
            "sht_Marks_AB",
            "sht_Marks_C",
            "sht_Marks_D",
            "sht_Packages",
            "sht_Sheets_AB",
            "sht_Sheets_C",
            "sht_Sheets_D",
            "sht_Sheets_R",
            "rbd_AuditoriumParallels",
            "rbd_ExpertParallels",
            "rbd_StationParallels",
            "rbd_StationWorkerParallels"
        };
        public static Dictionary<string, string> TABLES_INFO = new Dictionary<string, string>()
        {
            { "ac_Appeals", "Таблица содержит информацию об апелляциях." },
            { "ac_AppealTasks", "Таблица содержит информацию об измененных заданиях по апелляции." },
            { "ac_AppealDecisions", "Таблица содержит информацию о данных решения конфликтной комиссии." },
            { "ac_AppealExperts", "Данные эксперта привлеченного для рассмотрения апелляции." },
            { "ac_Changes", "Изменение тестового балла, к которому привело применение апелляции." },
            { "dats_Borders", "шкалы ГИА-9"},
            { "dats_Groups", "группы критериев функционала шкалирования" },
            { "prnf_CertificatePrintMain", "Данные о справках ГИА-9" },
            { "rbd_Address", "Адреса объектов (РЦОИ, ОО, ППЭ)." },
            { "rbd_Areas", "Справочник административно-территориальных единиц." },
            { "rbd_Auditoriums", "Справочник аудиторий ППЭ." },
            { "rbd_AuditoriumsSubjects", "Предметная специализация аудиторий" },
            { "rbd_CurrentRegion", "Информация о субъекте РФ" },
            { "rbd_CurrentRegionAddress", "Таблица для связи адресов с РЦОИ." },
            { "rbd_Experts", "Данные об экспертах" },
            { "rbd_ExpertsExams", "Распределение экспертов по экзаменам" },
            { "rbd_ExpertsSubjects", "Предметная специализация экспертов" },
            { "rbd_Governments", "Справочник ОИВ субъекта РФ" },
            { "rbd_ParticipantProperties", "Данные о параметрах участников" },
            { "rbd_Participants", "Список участников ГИА" },
            { "rbd_ParticipantClasses", "Информация о привязке участников к классам" },
            { "rbd_ParticipantsExamPStation", "Данные об автоматизированном распределении участников по  аудиториям ППЭ" },
            { "rbd_ParticipantsExams", "Данные о выборе экзаменов участниками" },
            { "rbd_ParticipantsExamsOnStation", "Данные о распределении участников по ППЭ" },
            { "rbd_ParticipantsProfSubject", "Профильные предметы участников" },
            { "rbd_ParticipantsSubject", "Список заявлений участников ГИА по предметам" },
            { "rbd_Places", "Справочник мест в аудиториях ППЭ" },
            { "rbd_SchoolAddress", "Адреса школ" },
            { "rbd_Schools", "Список школ" },
            { "rbd_SchoolClasses", "Справочник классов" },
            { "rbd_StationExamAuditory", "Данные о назначении аудиторий на экзамен" },
            { "rbd_StationExamAuditoryProps", "Дополнительные параметры аудиторий, распределённых на экзамены" },
            { "rbd_StationForm", "Данные по форме 13-02 МАШ" },
            { "rbd_StationFormAct", "Данные по форме 18-МАШ" },
            { "rbd_StationFormAuditoryFields", "Данные по форме 13-02 МАШ" },
            { "rbd_StationFormFields", "Данные по форме 13-02 МАШ" },
            { "rbd_Stations", "Справочник ППЭ" },
            { "rbd_StationsExams", "Распределение ППЭ по экзаменам" },
            { "rbd_StationWorkerOnExam", "Распределение работника на экзамен в ППЭ" },
            { "rbd_StationWorkerOnStation", "Прикрепление работника к ППЭ" },
            { "rbd_StationWorkers", "Данные о работниках ППЭ." },
            { "rbd_StationWorkersAccreditation", "данные об аккредитации." },
            { "rbd_StationWorkersSubjects", "Предметная специализация организаторов ГИА." },
            { "rbd_StationWorkersPositions", "Данные о возможных должностях работников ППЭ." },
            { "res_Answers", "Ответы участника на задания КИМ." },
            { "res_Complects", "Связки бланков по штрих - кодам в комплект." },
            { "res_HumanTests", "Человеко-тесты" },
            { "res_Marks", "Оценки" },
            { "res_SubComplects", "Данные о комплектах устной части иностранных языков" },
            { "res_SubTests", "Данные о человеко-тестах устной части и детальной математики" },
            { "sht_Alts", "Протоколы у экспертов" },
            { "sht_FinalMarks_C", "Окончательные оценки по письменной части тестирования" },
            { "sht_FinalMarks_D", "Данные об окончательных оценках по устной части тестирования" },
            { "sht_Marks_AB", "Ответы на бланках №1" },
            { "sht_Marks_C", "Оценки, выставленные экспертами, по письменной части тестирования" },
            { "sht_Marks_D", "Данные об оценках, выставленных экспертами, по устной части тестирования" },
            { "sht_Packages", "Пакеты с обработанными бланками всех типов" },
            { "sht_Sheets_AB", "Данные с бланков ответов №1 – регистрационная часть" },
            { "sht_Sheets_C", "Бланки по письменной части тестирования" },
            { "sht_Sheets_D", "Данные о бланках по устной части тестирования" },
            { "sht_Sheets_R", "Данные с бланков ответов №1 – персональные данные" },
            { "ac_Corrections", "Таблица содержит информацию о коррекциях на паспортные данные." },
            { "rbd_ParticipantsExamsOnSchool", "Данные по назначениям на итоговое собеседование участников в ОО." },
            { "rbd_RegionFiles", "Таблица содержит загруженные файлы с рассадкой на ГВЭ." },
            { "rbd_SchoolParticipant", "Таблица содержит данные о привязке участников ГИА к ОО." },
            { "sht_ExamFinish", "Таблица содержит информацию о завершении обработки экзаменов." },
            { "rbd_AuditoriumParallels", "Таблица содержит привязку карточки аудитории к параллели ГИА."},
            { "rbd_ExpertParallels", "Таблица содержит привязку карточки эксперта к параллели ГИА."},
            { "rbd_StationParallels", "Таблица содержит привязку карточки ППЭ к параллели ГИА."},
            { "rbd_StationWorkerParallels", "Таблица содержит привязку карточки работника ППЭ к параллели ГИА."}
        };

        public static Dictionary<string, int> DependencyMap = new Dictionary<string, int>
        {
            { "ac_AppealDecisions", 0 },
            { "ac_AppealExperts", 0 },
            { "ac_AppealTasks", 0 },
            { "ac_Changes", 0 },
            { "dats_Borders", 0 },
            { "dats_Groups", 0 },
            { "prnf_CertificatePrintMain", 0 },
            { "rbd_StationFormAct", 0 },
            { "res_Complects", 0 },
            { "res_SubTests", 0 },
            { "sht_Alts", 0 },
            { "sht_ExamFinish", 0 },
            { "sht_FinalMarks_D", 0 },
            { "sht_Marks_D", 0 },
            { "ac_Appeals", 1 },
            { "ac_Corrections", 1 },
            { "rbd_RegionFiles", 1 },
            { "res_HumanTests", 1 },
            { "sht_Packages", 1 },
            { "sht_Sheets_C", 1 },
            { "rbd_Areas", 2 },
            { "rbd_CurrentRegion", 2 },
            { "res_Answers", 2 },
            { "res_Marks", 2 },
            { "sht_FinalMarks_C", 2 },
            { "sht_Marks_C", 2 },
            { "sht_Sheets_AB", 2 },
            { "sht_Sheets_D", 2 },
            { "sht_Sheets_R", 2 },
            { "rbd_Address", 3 },
            { "rbd_Governments", 3 },
            { "res_SubComplects", 3 },
            { "sht_Marks_AB", 3 },
            { "rbd_CurrentRegionAddress", 4 },
            { "rbd_Schools", 4 },
            { "rbd_Experts", 5 },
            { "rbd_Participants", 5 },
            { "rbd_SchoolAddress", 5 },
            { "rbd_SchoolClasses", 5 },
            { "rbd_Stations", 5 },
            { "rbd_StationWorkers", 5 },
            { "rbd_Auditoriums", 6 },
            { "rbd_ExpertsSubjects", 6 },
            { "rbd_ParticipantProperties", 6 },
            { "rbd_ParticipantsExams", 6 },
            { "rbd_ParticipantsProfSubject", 6 },
            { "rbd_ParticipantsSubject", 6 },
            { "rbd_SchoolParticipant", 6 },
            { "rbd_StationForm", 6 },
            { "rbd_StationsExams", 6 },
            { "rbd_StationWorkerOnStation", 6 },
            { "rbd_StationWorkersAccreditation", 6 },
            { "rbd_StationWorkersSubjects", 6 },
            { "rbd_StationWorkersPositions", 6 },
            { "rbd_ExpertParallels", 6 },
            { "rbd_StationParallels", 6 },
            { "rbd_StationWorkerParallels", 6 },
            { "rbd_AuditoriumsSubjects", 7 },
            { "rbd_ExpertsExams", 7 },
            { "rbd_ParticipantsExamsOnSchool", 7 },
            { "rbd_ParticipantClasses", 7 },
            { "rbd_ParticipantsExamsOnStation", 7 },
            { "rbd_Places", 7 },
            { "rbd_StationExamAuditory", 7 },
            { "rbd_StationFormAuditoryFields", 7 },
            { "rbd_StationFormFields", 7 },
            { "rbd_AuditoriumParallels", 7 },
            { "rbd_ParticipantsExamPStation", 8 },
            { "rbd_StationExamAuditoryProps", 8 },
            { "rbd_StationWorkerOnExam", 8 }
        };

        /// запрос построения таблицы зависимостей
        public static string DependancyQuery = @"
        with cc(
          ParentTableName,
          ParentColumn,
          DependantTableName,
          DependantTableColumn,
          IsNullable
        ) as (
          select
            SO_P.name as [ParentTableName],
            SC_P.name as [ParentColumn],
            SO_R.name as [DependantTableName],
            SC_R.name as [DependantTableColumn],
            SC_P.is_nullable as [IsNullable]
          from sys.foreign_key_columns FKC
          inner join sys.objects SO_P on SO_P.object_id = FKC.parent_object_id
          inner join sys.columns SC_P on (SC_P.object_id = FKC.parent_object_id)
            AND (SC_P.column_id = FKC.parent_column_id)
          inner join sys.objects SO_R on SO_R.object_id = FKC.referenced_object_id
          inner join sys.columns SC_R on (SC_R.object_id = FKC.referenced_object_id)
            AND (SC_R.column_id = FKC.referenced_column_id)
          where
            (
              (SO_P.name = '{0}')
              AND (SO_P.type = 'U')
            )
            OR (
              (SO_R.name = '{0}')
              AND (SO_R.type = 'U')
            )
            AND SO_R.name <> '{0}'
        )
        select
          *
        from cc
        where
          cc.DependantTableName like 'rbd[_]%'
          and UPPER(cc.ParentColumn) not like 'REGION'
          and cc.ParentTableName <> cc.DependantTableName";
    }
}
