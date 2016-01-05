Public NotInheritable Class clsTable

#Region "Private Functions"
    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Tables in DB. This function shall be called by 
    '                     public functions to create a table
    '**************************************************************************************************************
    Private Sub AddTables(ByVal strTab As String, ByVal strDesc As String, ByVal nType As SAPbobsCOM.BoUTBTableType)
        Dim oUserTablesMD As SAPbobsCOM.UserTablesMD
        Try
            oUserTablesMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
            'Adding Table
            If Not oUserTablesMD.GetByKey(strTab) Then
                oUserTablesMD.TableName = strTab
                oUserTablesMD.TableDescription = strDesc
                oUserTablesMD.TableType = nType
                If oUserTablesMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If
        Catch ex As Exception
            Throw ex
        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTablesMD)
            oUserTablesMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddFields
    'Parameter          : SstrTab As String,strCol As String,
    '                     strDesc As String,nType As Integer,i,nEditSize,nSubType As Integer
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Fields in DB Tables. This function shall be called by 
    '                     public functions to create a Field
    '**************************************************************************************************************
    Private Sub AddFields(ByVal strTab As String, _
                            ByVal strCol As String, _
                                ByVal strDesc As String, _
                                    ByVal nType As SAPbobsCOM.BoFieldTypes, _
                                        Optional ByVal i As Integer = 0, _
                                            Optional ByVal nEditSize As Integer = 10, _
                                                Optional ByVal nSubType As SAPbobsCOM.BoFldSubTypes = 0, _
                                                    Optional ByVal Mandatory As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO)
        Dim oUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            If Not (strTab = "OADM" Or strTab = "OACT" Or strTab = "OASC" Or strTab = "INV1" Or strTab = "ORCT" Or strTab = "OINV" Or strTab = "OPRC") Then
                strTab = "@" + strTab
            End If

            If Not IsColumnExists(strTab, strCol) Then
                oUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)

                oUserFieldMD.Description = strDesc
                oUserFieldMD.Name = strCol
                oUserFieldMD.Type = nType
                oUserFieldMD.SubType = nSubType
                oUserFieldMD.TableName = strTab
                oUserFieldMD.EditSize = nEditSize
                oUserFieldMD.Mandatory = Mandatory
                If oUserFieldMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldMD)

            End If

        Catch ex As Exception
            Throw ex
        Finally
            oUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    Public Sub addField(ByVal TableName As String, ByVal ColumnName As String, ByVal ColDescription As String, ByVal FieldType As SAPbobsCOM.BoFieldTypes, ByVal Size As Integer, ByVal SubType As SAPbobsCOM.BoFldSubTypes, ByVal ValidValues As String, ByVal ValidDescriptions As String, ByVal SetValidValue As String)
        Dim intLoop As Integer
        Dim strValue, strDesc As Array
        Dim objUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            strValue = ValidValues.Split(Convert.ToChar(","))
            strDesc = ValidDescriptions.Split(Convert.ToChar(","))
            If (strValue.GetLength(0) <> strDesc.GetLength(0)) Then
                Throw New Exception("Invalid Valid Values")
            End If

            If (Not IsColumnExists(TableName, ColumnName)) Then
                objUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)
                objUserFieldMD.TableName = TableName
                objUserFieldMD.Name = ColumnName
                objUserFieldMD.Description = ColDescription
                objUserFieldMD.Type = FieldType
                If (FieldType <> SAPbobsCOM.BoFieldTypes.db_Numeric) Then
                    objUserFieldMD.Size = Size
                Else
                    objUserFieldMD.EditSize = Size
                End If
                objUserFieldMD.SubType = SubType
                objUserFieldMD.DefaultValue = SetValidValue
                For intLoop = 0 To strValue.GetLength(0) - 1
                    objUserFieldMD.ValidValues.Value = strValue(intLoop)
                    objUserFieldMD.ValidValues.Description = strDesc(intLoop)
                    objUserFieldMD.ValidValues.Add()
                Next
                If (objUserFieldMD.Add() <> 0) Then
                    '  MsgBox(oApplication.Company.GetLastErrorDescription)
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldMD)
            Else


            End If

        Catch ex As Exception
            MsgBox(ex.Message)

        Finally


            objUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()

        End Try


    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : IsColumnExists
    'Parameter          : ByVal Table As String, ByVal Column As String
    'Return Value       : Boolean
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Function to check if the Column already exists in Table
    '**************************************************************************************************************
    Private Function IsColumnExists(ByVal Table As String, ByVal Column As String) As Boolean
        Dim oRecordSet As SAPbobsCOM.Recordset

        Try
            strSQL = "SELECT COUNT(*) FROM CUFD WHERE TableID = '" & Table & "' AND AliasID = '" & Column & "'"
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecordSet.DoQuery(strSQL)

            If oRecordSet.Fields.Item(0).Value = 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oRecordSet = Nothing
            GC.Collect()
        End Try
    End Function

    Private Sub AddKey(ByVal strTab As String, ByVal strColumn As String, ByVal strKey As String, ByVal i As Integer)
        Dim oUserKeysMD As SAPbobsCOM.UserKeysMD

        Try
            '// The meta-data object must be initialized with a
            '// regular UserKeys object
            oUserKeysMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserKeys)

            If Not oUserKeysMD.GetByKey("@" & strTab, i) Then

                '// Set the table name and the key name
                oUserKeysMD.TableName = strTab
                oUserKeysMD.KeyName = strKey

                '// Set the column's alias
                oUserKeysMD.Elements.ColumnAlias = strColumn
                oUserKeysMD.Elements.Add()
                oUserKeysMD.Elements.ColumnAlias = "RentFac"

                '// Determine whether the key is unique or not
                oUserKeysMD.Unique = SAPbobsCOM.BoYesNoEnum.tYES

                '// Add the key
                If oUserKeysMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

            End If

        Catch ex As Exception
            Throw ex

        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserKeysMD)
            oUserKeysMD = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try

    End Sub

    '********************************************************************
    'Type		            :   Function    
    'Name               	:	AddUDO
    'Parameter          	:   
    'Return Value       	:	Boolean
    'Author             	:	
    'Created Date       	:	
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To Add a UDO for Transaction Tables
    '********************************************************************
    Private Sub AddUDO(ByVal strUDO As String, ByVal strDesc As String, ByVal strTable As String, _
                                Optional ByVal sFind1 As String = "", Optional ByVal sFind2 As String = "", _
                                        Optional ByVal strChildTbl As String = "", Optional ByVal nObjectType As SAPbobsCOM.BoUDOObjType = SAPbobsCOM.BoUDOObjType.boud_Document)

        Dim oUserObjectMD As SAPbobsCOM.UserObjectsMD
        Try
            oUserObjectMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjectMD.GetByKey(strUDO) = 0 Then
                oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES

                If sFind1 <> "" And sFind2 <> "" Then
                    oUserObjectMD.FindColumns.ColumnAlias = sFind1
                    oUserObjectMD.FindColumns.Add()
                    oUserObjectMD.FindColumns.SetCurrentLine(1)
                    oUserObjectMD.FindColumns.ColumnAlias = sFind2
                    oUserObjectMD.FindColumns.Add()
                End If

                oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.LogTableName = ""
                oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.ExtensionName = ""

                If strChildTbl <> "" Then
                    oUserObjectMD.ChildTables.TableName = strChildTbl
                End If

                oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.Code = strUDO
                oUserObjectMD.Name = strDesc
                oUserObjectMD.ObjectType = nObjectType
                oUserObjectMD.TableName = strTable

                If oUserObjectMD.Add() <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If

        Catch ex As Exception
            Throw ex

        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD)
            oUserObjectMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try

    End Sub

#End Region

#Region "Public Functions"
    '*************************************************************************************************************
    'Type               : Public Function
    'Name               : CreateTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Creating Tables by calling the AddTables & AddFields Functions
    '**************************************************************************************************************
    Public Sub CreateTables()
        Dim oProgressBar As SAPbouiCOM.ProgressBar
        Try

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("OPRC", "Z_Brand", "Brand Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OPRC", "Z_Branch", "Branch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OPRC", "Z_CardCode", "Customer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OPRC", "Z_Warehouse", "Warehouse Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("OINV", "Z_CmpCode", "External Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OINV", "Z_InvNumber", "External Document Number", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("OINV", "Z_Branch", "Branch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OINV", "Z_ReportNo", "Report Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("OINV", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("ORCT", "Z_CmpCode", "External Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("ORCT", "Z_InvNumber", "External Document Number", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("ORCT", "Z_Branch", "Branch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("ORCT", "Z_ReportNo", "Report Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("ORCT", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)


            AddFields("INV1", "Z_LineNo", "Invoice Line Number", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("INV1", "Z_InvNumber", "External Invoice Number", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("JDT1", "Z_InvNumber", "External Document Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddTables("Z_TRANS", "Transaction Type", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_TRANS", "Z_TransCode", "SAP Transaction Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_TRANS", "Z_Debit", "Default Debit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_TRANS", "Z_Credit", "Default Credit Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)



            'AddFields("OACT", "Z_FuAcctcode", "Futura Account Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("OASC", "Z_Country", "Futura Country Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            'AddFields("OASC", "Z_Company", "Futura Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            '---- User Defined Object's
            AddTables("GIDN", "Goods In/Delivery Notes", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("GIDN", "Z_Type", "GIDN Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            AddFields("GIDN", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_GoodsNo", "Goods In Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDN", "Z_VatKey", "SAP VAT Code", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDN", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_AcctNo", "Account Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("GIDN", "Z_GoodsDate", "Goods In Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("GIDN", "Z_SupplierDocNo", "Supplier Document Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            AddFields("GIDN", "Z_GoodsBranch", "Goods In Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDN", "Z_Value", "Value(In Cost/Purchase Price)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("GIDN", "Z_PayDate", "Date of Payment", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("GIDN", "Z_PeriodPay", "Period to Pay", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDN", "Z_VatPercentage", "Percentage rate of VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("GIDN", "Z_SupplierType", "Supplier Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@GIDN", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("GIDNCO", "Goods In/Delivery Notes/Costs", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("GIDNCO", "Z_Type", "GIDNCO Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            AddFields("GIDNCO", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDNCO", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDNCO", "Z_GoodsNo", "Goods In Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDNCO", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("GIDNCO", "Z_Accounttxt", "Account Text", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("GIDNCO", "Z_VatKey", "SAP VAT Code", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDNCO", "Z_NetValue", "Net Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("GIDNCO", "Z_VatPercentage", "Percentage rate of VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("GIDNCO", "Z_SupplierType", "Supplier Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@GIDNCO", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
         
            ' AddTables("GIIV", "Goods In/Supplier Invoices", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("GIIV", "Z_Type", "GIIV Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("GIIV", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIV", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIV", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIV", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIV", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIV", "Z_SupplierInvNo", "Supplier Invoice Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIV", "Z_InvoiceDate", "Invoice Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("GIIV", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("GIIV", "Z_TotalGrAmt", "Total Gross Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@GIIV", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("GIIVLG", "Goods In/Supp.Inv/Pos.Costs", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("GIIVLG", "Z_Type", "GIIVLG Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("GIIVLG", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLG", "Z_LineNo", "Line Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLG", "Z_GoodsBranch", "Goods In Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_GoodsNo", "Goods In Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLG", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLG", "Z_SupplierDocNo", "Supplier Document Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLG", "Z_Accounttxt", "Account Text", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("GIIVLG", "Z_NetValue", "Net Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@GIIVLG", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("GIIVLC", "Goods In/Supp.Inv/Pos.Costs", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("GIIVLC", "Z_Type", "GIIVLC Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("GIIVLC", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLC", "Z_LineNo", "Line Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLC", "Z_GoodsBranch", "Goods In Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_GoodsNo", "Goods In Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLC", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("GIIVLC", "Z_SupplierDocNo", "Supplier Document Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("GIIVLC", "Z_Accounttxt", "Account Text", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("GIIVLC", "Z_NetValue", "Net Value", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@GIIVLC", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            ' AddTables("IBTDN", "Inter Branch Transfers", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("IBTDN", "Z_Type", "IBTDN Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("IBTDN", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_DeliveryNoteNo", "Delivery Note Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("IBTDN", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_DeliveryNoteDt", "Delivery Note Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("IBTDN", "Z_ToBranch", "To Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTDN", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' AddFields("IBTDN", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("IBTDN", "Z_DelNotePrice", "Delivery Note Purchase Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' addField("@IBTDN", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' AddTables("IBTCO", "Inter Branch Transfer Confir.", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("IBTCO", "Z_Type", "IBTCO Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("IBTCO", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTCO", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTCO", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("IBTCO", "Z_DeliveryNoteNo", "Delivery Note Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("IBTCO", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            '  AddFields("IBTCO", "Z_ToBranch", "To Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("IBTCO", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' AddFields("IBTCO", "Z_DateofConf", "Date of Confirmation", SAPbobsCOM.BoFieldTypes.db_Date)
            ' addField("@IBTCO", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' AddTables("SURDN", "Supplier Return(Delivery Note)", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SURDN", "Z_Type", "SURDN Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SURDN", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURDN", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURDN", "Z_DeliveryNoteNo", "Delivery Note Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURDN", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURDN", "Z_ToSupplier", "To Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURDN", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURDN", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURDN", "Z_DeliveryNoteDt", "Delivery Note Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SURDN", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("SURDN", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' AddFields("SURDN", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("SURDN", "Z_DelNotePrice", "Delivery Note Purchase Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_SalePriceNet", "Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_SalePriceVat", "Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_OSalePriceNet", "Original Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_OSalePriceVat", "Original Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_PerRateofVAT", "Percentage rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURDN", "Z_SupplierType", "Supplier Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@SURDN", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            ' AddTables("CUSDN", "Customer Deliver Note", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("CUSDN", "Z_Type", "CUSDN Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("CUSDN", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSDN", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSDN", "Z_DeliveryNoteNo", "Delivery Note Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CUSDN", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CUSDN", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSDN", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            '  AddFields("CUSDN", "Z_Account", "Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSDN", "Z_DeliveryNoteDt", "Delivery Note Date", SAPbobsCOM.BoFieldTypes.db_Date)

            ' AddFields("CUSDN", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSDN", "Z_ToCustomer", "To Customer", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("CUSDN", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' AddFields("CUSDN", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("CUSDN", "Z_DelNotePrice", "Delivery Note Purchase Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_SalePriceNet", "Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_SalePriceVat", "Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_OSalePriceNet", "Original Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_OSalePriceVat", "Original Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_PerRateofVAT", "Percentage rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSDN", "Z_CustomerType", "Customer Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@CUSDN", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SURIV", "Supplier Return", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SURIV", "Z_Type", "SURIV Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SURIV", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIV", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIV", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIV", "Z_InvDate", "Invoice Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SURIV", "Z_DeliveryNote", "Delivery Note", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("SURIV", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIV", "Z_ToSupplier", "To Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIV", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' addField("@SURIV", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            'oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            ' AddTables("SURIVLG", "Supplier Return Positions", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SURIVLG", "Z_Type", "SURIVLG Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SURIVLG", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLG", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLG", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLG", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("SURIVLG", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLG", "Z_Account", "Account (Goods In)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLG", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLG", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("SURIVLG", "Z_DelNotePrice", "Delivery Note Purchase Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURIVLG", "Z_SalePriceNet", "Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURIVLG", "Z_SalePriceVat", "Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURIVLG", "Z_OSalePriceNet", "Original Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURIVLG", "Z_OSalePriceVat", "Original Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("SURIVLG", "Z_PerRateofVAT", "Percentage rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' addField("@SURIVLG", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SURIVLC", "Supplier Return Costs", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SURIVLC", "Z_Type", "SURIVLC Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SURIVLC", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLC", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SURIVLC", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_FreightPer", "Freight Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            ' AddFields("SURIVLC", "Z_FreightNet", "Freight Net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_FreightVat", "Freight Incl.Vat", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_TransInsPer", "Transport Insurance Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            ' AddFields("SURIVLC", "Z_TransInsNet", "Transport Insurance Net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_TransInsVat", "Transport Insurance incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_TransCostNet", "Transport Costs Net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SURIVLC", "Z_TransCostVat", "Transport Costs Incl.Vat", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@SURIVLC", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("CUSIV", "Customer Credit Invoice", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("CUSIV", "Z_Type", "CUSIV Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("CUSIV", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_Originator", "Originator", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_InvoiceNo", "Invoice Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CUSIV", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CUSIV", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_Account", "Account (Turn Over)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("CUSIV", "Z_InvDate", "Invoice Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("CUSIV", "Z_DeliveryNote", "Delivery Note", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_FromBranch", "From (Branch)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_ToCustomer", "To Customer", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CUSIV", "Z_Comment", "Comment", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            ' AddFields("CUSIV", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("CUSIV", "Z_DelNotePrice", "Delivery Note Purchase Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_SalePriceNet", "Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_SalePriceVat", "Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_OSalePriceNet", "Original Sales price net", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_OSalePriceVat", "Original Sales price Incl.VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_PerRateofVAT", "Percentage rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("CUSIV", "Z_CustomerType", "Customer Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@CUSIV", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("STKCOR", "Stock Correction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("STKCOR", "Z_Type", "STKCOR Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("STKCOR", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKCOR", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKCOR", "Z_DocId", "DocId", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKCOR", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("STKCOR", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKCOR", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKCOR", "Z_Account", "Account (Goods In)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("STKCOR", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("STKCOR", "Z_Reason", "Reason", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("STKCOR", "Z_Reasontxt", "ReasonText", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("STKCOR", "Z_PurCostPrice", "Purchase/Cost Price(Value)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("STKCOR", "Z_SalesPrice", "Sales price (Value)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("STKCOR", "Z_PerRateVat", "Percentage Rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("STKCOR", "Z_DateCorre", "Date of Correction", SAPbobsCOM.BoFieldTypes.db_Date)
            ' addField("@STKCOR", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            ' AddTables("STKTAK", "Stock Take", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("STKTAK", "Z_Type", "STKTAK Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("STKTAK", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKTAK", "Z_InventoryNo ", "Inventory number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("STKTAK", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("STKTAK", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKTAK", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKTAK", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("STKTAK", "Z_Account", "Account (Goods In)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ' AddFields("STKTAK", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("STKTAK", "Z_Text", "Text", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("STKTAK", "Z_ExpQuantity", "Expected Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("STKTAK", "Z_InvDiff", "Inventory Differences", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("STKTAK", "Z_ProQuantity", "Processed Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            ' AddFields("STKTAK", "Z_PurCostPrice", "Purchase/Cost Price(Value)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("STKTAK", "Z_SalesPrice", "Sales price (Value)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            ' AddFields("STKTAK", "Z_PerRateVat", "Percentage Rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@STKTAK", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("SLSTRN", "Sales TurnOver", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("SLSTRN", "Z_Type", "SLSTRN Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            AddFields("SLSTRN", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSTRN", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("SLSTRN", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSTRN", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("SLSTRN", "Z_VatKey", "VAT Key", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_Supplier", "Supplier", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSTRN", "Z_Brand", "External Item Attribute", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSTRN", "Z_Account", "Account (Turn Over)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("SLSTRN", "Z_Sales", "Sales", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_Discount", "Discounts", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_Vat", "VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_CostPrice", "Cost Price", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_Quantity", "Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_PerRateVat", "Percentage Rate of the VAT", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSTRN", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            addField("@SLSTRN", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddTables("SLSPAY", "Incoming Payments", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("SLSPAY", "Z_Type", "SLSPAY Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            AddFields("SLSPAY", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSPAY", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("SLSPAY", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSPAY", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSPAY", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("SLSPAY", "Z_PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            AddFields("SLSPAY", "Z_Account", "Account (Payments)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSPAY", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            AddFields("SLSPAY", "Z_TurnOver ", "TurnOver", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSPAY", "Z_TurnFrgCur ", "TurnOver Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSPAY", "Z_FrgCurrency", "Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            addField("@SLSPAY", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddTables("SLSDIF", "Sales Differences", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("SLSDIF", "Z_Type", "SLSDIF Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            AddFields("SLSDIF", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSDIF", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("SLSDIF", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSDIF", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSDIF", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSDIF", "Z_PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            AddFields("SLSDIF", "Z_Account", "Account (Differences)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("SLSDIF", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            AddFields("SLSDIF", "Z_Difference", "Differences", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSDIF", "Z_DiffFrgCur ", "Differences Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("SLSDIF", "Z_FrgCurrency", "Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            addField("@SLSDIF", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SLSDRP", "Sales Drop off Not Applicable", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SLSDRP", "Z_Type", "SLSDRP Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SLSDRP", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSDRP", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SLSDRP", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSDRP", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSDRP", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSDRP", "Z_PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            ' AddFields("SLSDRP", "Z_Account", "Account (Payment Type)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSDRP", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("SLSDRP", "Z_DropAmt", "Drop Off Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSDRP", "Z_DropFrgCur ", "Drop Off Amount Forg.Currency", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSDRP", "Z_FrgCurrency", "Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' addField("@SLSDRP", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            ' AddTables("SLSPUP", " Sales Pick Up Not Applicable", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SLSPUP", "Z_Type", "SLSPUP Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SLSPUP", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPUP", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SLSPUP", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPUP", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPUP", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPUP", "Z_PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            ' AddFields("SLSPUP", "Z_Account", "Account (Payment Type)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPUP", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("SLSPUP", "Z_PickAmt", "Pick Up Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPUP", "Z_PickFrgCur ", "Pick Up Amount Forg.Currency", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPUP", "Z_FrgCurrency", "Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' addField("@SLSPUP", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SLSGFT", " Sales Sold Gift Vouchers", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SLSGFT", "Z_Type", "SLSGFT Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SLSGFT", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSGFT", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SLSGFT", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSGFT", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSGFT", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSGFT", "Z_PayType", "Pay.Type(Gift Voucher Sales)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            ' AddFields("SLSGFT", "Z_Account", "Account (Gift Voucher Sales)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSGFT", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            '  AddFields("SLSGFT", "Z_GiftAmt", "Gift Voucher Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSGFT", "Z_GiftFrgCur ", "Gift Voucher Foreign Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSGFT", "Z_FrgCurrency", "Foreign Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' addField("@SLSGFT", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SLSPRE", "Prepayments Not Applicable", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SLSPRE", "Z_Type", "SLSPRE Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SLSPRE", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPRE", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SLSPRE", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPRE", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPRE", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSPRE", "Z_PayType", "Pay.Type(PrePayment)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            ' AddFields("SLSPRE", "Z_Account", "Account (PrePayment)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSPRE", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            '  AddFields("SLSPRE", "Z_PrePayAmt", "PrePayment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@SLSPRE", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("SLSEXP", "Sales Expenses", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("SLSEXP", "Z_Type", "SLSEXP Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("SLSEXP", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSEXP", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("SLSEXP", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSEXP", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSEXP", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("SLSEXP", "Z_ExpType", "Expenses Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)

            ' AddFields("SLSEXP", "Z_Account", "Account (Expenses)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("SLSEXP", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("SLSEXP", "Z_ExpAmt", "Expenses Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@SLSEXP", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            ' AddTables("CREBAL", "Credit Balance.", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            ' AddFields("CREBAL", "Z_Type", "CREBAL Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 6)
            ' AddFields("CREBAL", "Z_CompanyCode", "ExtCompany Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CREBAL", "Z_Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' AddFields("CREBAL", "Z_Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CREBAL", "Z_ReportNo ", "Report number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CREBAL", "Z_CashNo", "Cash Number", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CREBAL", "Z_PayType", "Payment Type(Credit Balance)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("CREBAL", "Z_Account", "Account (Credit Balance)", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CREBAL", "Z_Customer", "Customer", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            ' AddFields("CREBAL", "Z_CustomerType", "Customer Type", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' AddFields("CREBAL", "Z_RefDoc", "Referring Document", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            ' AddFields("CREBAL", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 3)
            ' AddFields("CREBAL", "Z_BalAmt", "Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            ' addField("@CREBAL", "Z_Imported", "Imported", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            CreateUDO()

            oApplication.Utilities.Message("Initializing Database Completed...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

        Catch ex As Exception
            Throw ex
        Finally
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Sub

    Public Sub CreateUDO()
        Try

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

End Class
