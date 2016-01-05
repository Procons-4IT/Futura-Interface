Public Class clsBranchMapping
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Private Sub LoadForm()
        oApplication.Utilities.LoadForm(xml_BranchMapping, frm_BranchMapping)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oGrid = oForm.Items.Item("1").Specific
        ' oForm.EnableMenu(mnu_ADD_ROW, True)
        ' oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        oForm.Freeze(True)
        AddtoUDT("GIDN", "Good in Delivery")
        AddtoUDT("SLSTRN", "Sales Turnover")
        AddtoUDT("SLSPAY", "Sales Payment")
        AddtoUDT("SLSDIF", "Sales Difference")
        FormatGrid(oGrid)
        oForm.Freeze(False)
    End Sub

    Private Sub AddtoUDT(ByVal code As String, ByVal name As String)
        Dim oUsertable As SAPbobsCOM.UserTable
        Dim strsql As String
        oUsertable = oApplication.Company.UserTables.Item("Z_TRANS")
        If oUsertable.GetByKey(code) = False Then
            oUsertable.Code = code
            oUsertable.Name = name
            If oUsertable.Add <> 0 Then
            End If
        End If
    End Sub

    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFL = oCFLs.Item("CFL_1")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub FormatGrid(ByVal aGrid As SAPbouiCOM.Grid)
        oGrid.DataTable.ExecuteQuery("select *  from [@Z_TRANS] order by Code")
        oGrid.Columns.Item("Code").TitleObject.Caption = "Transaction Code"
        oGrid.Columns.Item("Name").TitleObject.Caption = "Transaction Name"
        oGrid.Columns.Item("Code").Editable = False
        oGrid.Columns.Item("Name").Editable = False
        oGrid.Columns.Item("U_Z_TransCode").TitleObject.Caption = "SAP Transaction Code"
        oGrid.Columns.Item("U_Z_TransCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item("U_Z_TransCode")
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otest.DoQuery("SElect * from OTRC")
        For intRow As Integer = 0 To otest.RecordCount - 1
            oComboColumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            otest.MoveNext()
        Next
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oGrid.Columns.Item("U_Z_Debit").TitleObject.Caption = "Default Debit Account"
        oEditTextColumn = oGrid.Columns.Item("U_Z_Debit")
        oEditTextColumn.ChooseFromListUID = "CFL_1"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn.LinkedObjectType = 1
        oGrid.Columns.Item("U_Z_Credit").TitleObject.Caption = "Default Credit Account"
        oEditTextColumn = oGrid.Columns.Item("U_Z_Credit")
        oEditTextColumn.ChooseFromListUID = "CFL_2"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn.LinkedObjectType = 1
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None
    End Sub

    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_CODE", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
    Private Sub addtoUDT(ByVal aform As SAPbouiCOM.Form)
        Dim oRec As SAPbobsCOM.Recordset
        oGrid = aform.Items.Item("1").Specific
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oUsertable As SAPbobsCOM.UserTable
        Dim strsql As String
        oUsertable = oApplication.Company.UserTables.Item("Z_TRANS")
        Dim strTranscode As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oComboColumn = oGrid.Columns.Item("U_Z_TransCode")
            Try
                strTranscode = oComboColumn.GetSelectedValue(intRow).Value
            Catch ex As Exception
                strTranscode = ""

            End Try

            If oUsertable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                oUsertable.Code = oGrid.DataTable.GetValue("Code", intRow)
                oUsertable.Name = oGrid.DataTable.GetValue("Name", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_TransCode").Value = strTranscode ' oComboColumn.GetSelectedValue(intRow).Value ' oGrid.DataTable.GetValue("U_Z_TransCode", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_Debit").Value = oGrid.DataTable.GetValue("U_Z_Debit", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_Credit").Value = oGrid.DataTable.GetValue("U_Z_Credit", intRow)
                If oUsertable.Update <> 0 Then
                    MsgBox(oApplication.Company.GetLastErrorDescription)
                End If
            Else
                oUsertable.Code = oGrid.DataTable.GetValue("Code", intRow)
                oUsertable.Name = oGrid.DataTable.GetValue("Name", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_TransCode").Value = strTranscode ' oComboColumn.GetSelectedValue(intRow).Value 'oGrid.DataTable.GetValue("U_Z_TransCode", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_Debit").Value = oGrid.DataTable.GetValue("U_Z_Debit", intRow)
                oUsertable.UserFields.Fields.Item("U_Z_Credit").Value = oGrid.DataTable.GetValue("U_Z_Credit", intRow)
                If oUsertable.Add <> 0 Then
                    MsgBox(oApplication.Company.GetLastErrorDescription)
                End If
            End If
        Next
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_BranchMapping Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    addtoUDT(oForm)
                                    FormatGrid(oGrid)
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
                                Dim intChoice As Integer
                                Dim codebar As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oGrid = oForm.Items.Item("1").Specific
                                        intChoice = 0
                                        oForm.Freeze(True)
                                        If ((pVal.ItemUID = "1" And (pVal.ColUID = "U_Z_Debit" Or pVal.ColUID = "U_Z_Credit"))) Then
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        ElseIf pVal.ItemUID = "1" And (pVal.ColUID = "U_Z_warehouse" Or pVal.ColUID = "U_Z_CardCode") Then
                                            If pVal.ColUID = "U_Z_CardCode" Then
                                                val = oDataTable.GetValue("CardCode", 0)
                                            Else
                                                val = oDataTable.GetValue("WhsCode", 0)
                                            End If

                                            oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)

                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oForm.Freeze(False)
                                End Try

                        End Select
                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_Branch
                    If pVal.BeforeAction = False Then
                        LoadForm()
                    End If
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                    End If

                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
