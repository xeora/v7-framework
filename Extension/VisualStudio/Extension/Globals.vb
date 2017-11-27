Option Strict On

Namespace Xeora.Extension.VisualStudio
    Public Class Globals
        Public Enum ControlTypes
            Textbox
            Password
            Checkbox
            Button
            RadioButton
            Textarea
            ImageButton
            LinkButton

            DataList
            ConditionalStatement
            VariableBlock

            Unknown
        End Enum

        Enum ActiveDomainTypes
            Domain
            Child
        End Enum

        Public Enum SpecialPropertyTypes
            [Single]
            Block
            Control

            Unknown
        End Enum

        Public Enum ISTypes
            ControlSearch = 0
            TemplateSearch = 1
            AssemblySearch = 2
            ClassSearch = 3
            FunctionSearch = 4
            TranslationSearch = 5
            TypeSearch = 6
            OnFlyRequest = 7
            PrimitiveStatement = 8
            SpecialPropertySearch = 9
            ControlSearchForParenting = 10
        End Enum

        Public Shared Function ParseControlType(ByVal cTString As String) As Globals.ControlTypes
            Dim rControlType As Globals.ControlTypes = Globals.ControlTypes.Unknown

            Dim ControlTypeNames As String() =
                    [Enum].GetNames(GetType(Globals.ControlTypes))

            For Each ControlTypeName As String In ControlTypeNames
                If String.Compare(ControlTypeName, cTString, True, New System.Globalization.CultureInfo("en-US")) = 0 Then
                    rControlType = CType(
                                        [Enum].Parse(
                                            GetType(Globals.ControlTypes),
                                            ControlTypeName,
                                            True
                                        ),
                                        Globals.ControlTypes)

                    Exit For
                End If
            Next

            Return rControlType
        End Function
    End Class
End Namespace
