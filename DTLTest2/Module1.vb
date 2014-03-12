﻿Imports DTL.DTL.SimulationObjects.PropertyPackages

Module Module1

    Sub Main()

        RunTest()

    End Sub

    Sub RunTest()

        Dim dtlc As New DTL.Thermodynamics.Calculator()
        dtlc.Initialize()

        Dim proppacks As String() = dtlc.GetPropPackList()
        Dim prpp As PropertyPackage = dtlc.GetPropPackInstance(proppacks(3))

        prpp.StabilityTestKeyCompounds = New String() {"Water"}
        prpp.StabilityTestSeverity = 0
        prpp._ioquick = False

        Dim P As Double = 9
        Dim T As Double = 94

        Dim result2 As Object(,) = dtlc.PTFlash(prpp, 3, P * 101325, T + 273.15,
                                                New String() {"Water", "Ethane", "Propane", "Isobutane", "N-butane", "1-butene", "N-pentane"},
                                                New Double() {0.048, 0.047, 0.402, 0.264, 0.23, 0.008, 0.003})

        Console.Write(vbCrLf)
        Console.WriteLine("Flash calculation results at P = " & P & " bar and T = " & T & " C:")
        Console.Write(vbCrLf)

        Dim i, j As Integer, line As String
        For i = 0 To result2.GetLength(0) - 1
            Select Case i
                Case 0
                    line = "Phase Name".PadRight(24) & "Mixture" & vbTab
                Case 1
                    line = "Phase Mole Fraction".PadRight(24) & vbTab
                Case 2
                    line = "Water".PadRight(24) & "0.0480" & vbTab
                Case 3
                    line = "Ethane".PadRight(24) & "0.0470" & vbTab
                Case 4
                    line = "Propane".PadRight(24) & "0.4020" & vbTab
                Case 5
                    line = "Isobutane".PadRight(24) & "0.2640" & vbTab
                Case 6
                    line = "N-butane".PadRight(24) & "0.2300" & vbTab
                Case 7
                    line = "1-butene".PadRight(24) & "0.0080" & vbTab
                Case 8
                    line = "N-pentane".PadRight(24) & "0.0030" & vbTab
                Case Else
                    line = ""
            End Select
            For j = 0 To result2.GetLength(1) - 1
                If Double.TryParse(result2(i, j).ToString, New Double) Then
                    line += Double.Parse(result2(i, j).ToString).ToString("0.0000").PadRight(10)
                Else
                    line += result2(i, j).ToString.PadRight(10)
                End If
            Next
            Console.WriteLine(line)
        Next

        Console.Write(vbCrLf)
        Console.WriteLine("Press any key to continue...")
        Console.ReadKey(True)

    End Sub


End Module
