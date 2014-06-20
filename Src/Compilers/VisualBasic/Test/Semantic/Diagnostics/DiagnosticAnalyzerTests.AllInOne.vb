' Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Test.Utilities

Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class DiagnosticAnalyzerTests
        <WorkItem(897137, "DevDiv")>
        <Fact>
        Public Sub DiagnosticAnalyzerAllInOne()
            Dim source = TestResource.AllInOneVisualBasicBaseline
            Dim analyzer = New BasicTrackingDiagnosticAnalyzer()
            CreateCompilationWithMscorlib({source}).VerifyAnalyzerDiagnostics({analyzer})
            analyzer.VerifyAllInterfaceMembersWereCalled()
            analyzer.VerifyAnalyzeSymbolCalledForAllSymbolKinds()
            analyzer.VerifyAnalyzeNodeCalledForAllSyntaxKinds()
            analyzer.VerifyOnCodeBlockCalledForAllSymbolAndMethodKinds()
        End Sub

        <WorkItem(896273, "DevDiv")>
        <Fact>
        Public Sub DiagnosticAnalyzerEnumBlock()
            Dim source =
<project><file><![CDATA[
Public Enum E
    Zero
    One
    Two
End Enum
]]></file></project>
            CreateCompilationWithMscorlib(source).VerifyAnalyzerDiagnostics({New BasicTrackingDiagnosticAnalyzer()})
        End Sub

        <Fact>
        Public Sub AnalyzerDriverIsSafeAgainstAnalyzerExceptions()
            Dim compilation = CreateCompilationWithMscorlib({TestResource.AllInOneVisualBasicCode})
            ThrowingDiagnosticAnalyzer(Of SyntaxKind).VerifyAnalyzerEngineIsSafeAgainstExceptions(
                Function(analyzer) AnalyzerDriver.GetDiagnostics(compilation, {analyzer}, Nothing, CancellationToken.None), GetType(AnalyzerDriver).Name)
        End Sub

        <Fact>
        Public Sub AnalyzerOptionsArePassedToAllAnalyzers()

            Dim options = New AnalyzerOptions({New AdditionalFileStream("myfilepath")},
                                              New Dictionary(Of String, String) From {{"optionName", "optionValue"}})

            Dim compilation = CreateCompilationWithMscorlib({TestResource.AllInOneVisualBasicCode})
            Dim analyzer = New OptionsDiagnosticAnalyzer(Of SyntaxKind)(options)
            AnalyzerDriver.GetDiagnostics(compilation, {analyzer}, options, CancellationToken.None)
            analyzer.VerifyAnalyzerOptions()

            ' TODO: Repeat with AnalyzerDriver3
        End Sub
    End Class
End Namespace
