﻿'    Peng-Robinson Property Package 
'    Copyright 2008 Daniel Wagner O. de Medeiros
'
'    This file is part of DTL.
'
'    DWSIM is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    DWSIM is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with DTL.  If not, see <http://www.gnu.org/licenses/>.

Imports DTL.DTL.SimulationObjects.PropertyPackages
Imports System.Math
Imports DTL.DTL.SimulationObjects.PropertyPackages.Auxiliary.FlashAlgorithms
Imports DTL.DTL.BaseThermoClasses

Namespace DTL.SimulationObjects.PropertyPackages

    <System.Runtime.InteropServices.Guid(PengRobinsonPropertyPackage.ClassId)> _
 <Serializable()> Public Class PengRobinsonPropertyPackage

        Inherits DTL.SimulationObjects.PropertyPackages.PropertyPackage

        Public Shadows Const ClassId As String = "2A322AB7-2256-495d-86C7-797AD19FDE22"

        Public MAT_KIJ(38, 38) As Object

        Private m_props As New Auxiliary.PROPS
        Public m_pr As New Auxiliary.PengRobinson

        Public Sub New(ByVal comode As Boolean)
            MyBase.New(comode)
        End Sub

        Public Sub New()

            MyBase.New()

            With Me.Parameters
                .Add("PP_USE_EOS_LIQDENS", 0)
            End With

            Me.IsConfigurable = True
            Me._packagetype = PackageType.EOS

        End Sub

        Public Overrides Sub ReconfigureConfigForm()
            MyBase.ReconfigureConfigForm()
        End Sub

#Region "    DWSIM Functions"

        Public Overrides Function DW_CalcCp_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double) As Double
            Select Case Phase1
                Case Phase.Liquid
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Phase.Aqueous
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Phase.Liquid1
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Phase.Liquid2
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Phase.Liquid3
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Phase.Vapor
                    Return Me.m_props.CpCvR("V", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
            End Select
        End Function

        Public Overrides Function DW_CalcMixtureEnergy_ISOL(ByVal T As Double, ByVal P As Double) As Double

            Dim HM, HV, HL As Double

            HL = Me.m_pr.H_PR_MIX("L", T, P, RET_VMOL(Phase.Liquid), RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Phase.Liquid))
            HV = Me.m_pr.H_PR_MIX("V", T, P, RET_VMOL(Phase.Vapor), RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Phase.Vapor))
            HM = Me.CurrentMaterialStream.Phases(1).SPMProperties.massfraction.GetValueOrDefault * HL + Me.CurrentMaterialStream.Phases(2).SPMProperties.massfraction.GetValueOrDefault * HV

            Dim ent_massica = HM
            Dim flow = Me.CurrentMaterialStream.Phases(0).SPMProperties.massflow
            Return ent_massica * flow

        End Function

        Public Overrides Function DW_CalcK_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double) As Double
            If Phase1 = Phase.Liquid Then
                Return Me.AUX_CONDTL(T)
            ElseIf Phase1 = Phase.Vapor Then
                Return Me.AUX_CONDTG(T, P)
            End If
        End Function

        Public Overrides Function DW_CalcSpecificMass_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double, Optional ByVal pvp As Double = 0) As Double
            If Phase1 = Phase.Liquid Then
                Return Me.AUX_LIQDENS(T)
            ElseIf Phase1 = Phase.Vapor Then
                Return Me.AUX_VAPDENS(T, P)
            ElseIf Phase1 = Phase.Mixture Then
                Return Me.CurrentMaterialStream.Phases(1).SPMProperties.volumetric_flow.GetValueOrDefault * Me.AUX_LIQDENS(T) / Me.CurrentMaterialStream.Phases(0).SPMProperties.volumetric_flow.GetValueOrDefault + Me.CurrentMaterialStream.Phases(2).SPMProperties.volumetric_flow.GetValueOrDefault * Me.AUX_VAPDENS(T, P) / Me.CurrentMaterialStream.Phases(0).SPMProperties.volumetric_flow.GetValueOrDefault
            End If
        End Function

        Public Overrides Function DW_CalcMM_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double) As Double
            Return Me.AUX_MMM(Phase1)
        End Function

        Public Overrides Sub DW_CalcOverallProps()
            MyBase.DW_CalcOverallProps()
        End Sub

        Public Overrides Sub DW_CalcProp(ByVal [property] As String, ByVal phase As Phase)

            Dim result As Double = 0.0#
            Dim resultObj As Object = Nothing
            Dim phaseID As Integer = -1
            Dim state As String = ""

            Dim T, P As Double
            T = Me.CurrentMaterialStream.Phases(0).SPMProperties.temperature
            P = Me.CurrentMaterialStream.Phases(0).SPMProperties.pressure

            Select Case phase
                Case Phase.Vapor
                    state = "V"
                Case Phase.Liquid, Phase.Liquid1, Phase.Liquid2, Phase.Liquid3
                    state = "L"
            End Select

            Select Case phase
                Case Phase.Mixture
                    phaseID = 0
                Case Phase.Vapor
                    phaseID = 2
                Case Phase.Liquid1
                    phaseID = 3
                Case Phase.Liquid2
                    phaseID = 4
                Case Phase.Liquid3
                    phaseID = 5
                Case Phase.Liquid
                    phaseID = 1
                Case Phase.Aqueous
                    phaseID = 6
                Case Phase.Solid
                    phaseID = 7
            End Select

            Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight = Me.AUX_MMM(phase)

            Select Case [property].ToLower
                Case "compressibilityfactor"
                    result = m_pr.Z_PR(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC, RET_VPC, RET_VW, state)
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.compressibilityFactor = result
                Case "heatcapacity", "heatcapacitycp"
                    resultObj = Me.m_props.CpCvR(state, T, P, RET_VMOL(phase), RET_VKij(), RET_VMAS(phase), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Case "heatcapacitycv"
                    resultObj = Me.m_props.CpCvR(state, T, P, RET_VMOL(phase), RET_VKij(), RET_VMAS(phase), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                Case "enthalpy", "enthalpynf"
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, phase))
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy = result
                    result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_enthalpy = result
                Case "entropy", "entropynf"
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, phase))
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy = result
                    result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_entropy = result
                Case "excessenthalpy"
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), 0)
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.excessEnthalpy = result
                Case "excessentropy"
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), 0)
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.excessEntropy = result
                Case "enthalpyf"
                    Dim entF As Double = Me.AUX_HFm25(phase)
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, phase))
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpyF = result + entF
                    result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpyF.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_enthalpyF = result
                Case "entropyf"
                    Dim entF As Double = Me.AUX_SFm25(phase)
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, phase))
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropyF = result + entF
                    result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropyF.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_entropyF = result
                Case "viscosity"
                    If state = "L" Then
                        result = Me.AUX_LIQVISCm(T)
                    Else
                        result = Me.AUX_VAPVISCm(T, Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.GetValueOrDefault, Me.AUX_MMM(phase))
                    End If
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.viscosity = result
                Case "thermalconductivity"
                    If state = "L" Then
                        result = Me.AUX_CONDTL(T)
                    Else
                        result = Me.AUX_CONDTG(T, P)
                    End If
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.thermalConductivity = result
                Case "fugacity", "fugacitycoefficient", "logfugacitycoefficient", "activity", "activitycoefficient"
                    Me.DW_CalcCompFugCoeff(phase)
                Case "volume", "density"
                    If state = "L" Then
                        result = Me.AUX_LIQDENS(T, P, 0.0#, phaseID, False)
                    Else
                        result = Me.AUX_VAPDENS(T, P)
                    End If
                    Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density = result
                Case Else
                    Dim ex As Exception = New NotImplementedException
                    ThrowCAPEException(ex, "Error", ex.Message, "ICapeThermoMaterial", ex.Source, ex.StackTrace, "CalcSinglePhaseProp/CalcTwoPhaseProp/CalcProp", ex.GetHashCode)
            End Select

        End Sub

        Public Overrides Sub DW_CalcPhaseProps(ByVal Phase As Phase)

            Dim result As Double
            Dim resultObj As Object
            Dim dwpl As Phase

            Dim T, P As Double
            Dim phasemolarfrac As Double = Nothing
            Dim overallmolarflow As Double = Nothing

            Dim phaseID As Integer
            T = Me.CurrentMaterialStream.Phases(0).SPMProperties.temperature
            P = Me.CurrentMaterialStream.Phases(0).SPMProperties.pressure

            Select Case Phase
                Case Phase.Mixture
                    phaseID = 0
                    dwpl = Phase.Mixture
                Case Phase.Vapor
                    phaseID = 2
                    dwpl = Phase.Vapor
                Case Phase.Liquid1
                    phaseID = 3
                    dwpl = Phase.Liquid1
                Case Phase.Liquid2
                    phaseID = 4
                    dwpl = Phase.Liquid2
                Case Phase.Liquid3
                    phaseID = 5
                    dwpl = Phase.Liquid3
                Case Phase.Liquid
                    phaseID = 1
                    dwpl = Phase.Liquid
                Case Phase.Aqueous
                    phaseID = 6
                    dwpl = Phase.Aqueous
                Case Phase.Solid
                    phaseID = 7
                    dwpl = Phase.Solid
            End Select

            If phaseID > 0 Then

                overallmolarflow = Me.CurrentMaterialStream.Phases(0).SPMProperties.molarflow.GetValueOrDefault
                phasemolarfrac = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molarfraction.GetValueOrDefault
                result = overallmolarflow * phasemolarfrac
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molarflow = result
                result = result * Me.AUX_MMM(Phase) / 1000
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.massflow = result
                result = phasemolarfrac * overallmolarflow * Me.AUX_MMM(Phase) / 1000 / Me.CurrentMaterialStream.Phases(0).SPMProperties.massflow.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.massfraction = result
                Me.DW_CalcCompVolFlow(phaseID)
                Me.DW_CalcCompFugCoeff(Phase)
            End If

            If phaseID = 3 Or phaseID = 4 Or phaseID = 5 Or phaseID = 6 Then

                If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                    Dim val As Double
                    val = m_pr.Z_PR(T, P, RET_VMOL(Phase), RET_VKij(), RET_VTC, RET_VPC, RET_VW, "L")
                    val = 1 / (8.314 * val * T / P)
                    val = val * Me.AUX_MMM(dwpl) / 1000
                    result = val
                Else
                    result = Me.AUX_LIQDENS(T, P, 0.0#, phaseID, False)
                End If

                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density = result

                result = Me.m_pr.H_PR_MIX("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, dwpl))
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy = result
                result = Me.m_pr.S_PR_MIX("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, dwpl))
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy = result
                result = Me.m_pr.Z_PR(T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC, RET_VPC, RET_VW, "L")
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.compressibilityFactor = result
                resultObj = Me.m_props.CpCvR("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VMAS(dwpl), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                result = Me.AUX_MMM(Phase)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight = result
                result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_enthalpy = result
                result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_entropy = result
                result = Me.AUX_CONDTL(T)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.thermalConductivity = result
                result = Me.AUX_LIQVISCm(T)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.viscosity = result
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.kinematic_viscosity = result / Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.Value

            ElseIf phaseID = 2 Then

                result = Me.AUX_VAPDENS(T, P)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density = result
                result = Me.m_pr.H_PR_MIX("V", T, P, RET_VMOL(Phase.Vapor), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, Phase.Vapor))
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy = result
                result = Me.m_pr.S_PR_MIX("V", T, P, RET_VMOL(Phase.Vapor), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, Phase.Vapor))
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy = result
                result = Me.m_pr.Z_PR(T, P, RET_VMOL(Phase.Vapor), RET_VKij, RET_VTC, RET_VPC, RET_VW, "V")
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.compressibilityFactor = result
                result = Me.AUX_CPm(Phase.Vapor, T)
                resultObj = Me.m_props.CpCvR("V", T, P, RET_VMOL(Phase.Vapor), RET_VKij(), RET_VMAS(Phase.Vapor), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                result = Me.AUX_MMM(Phase)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight = result
                result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_enthalpy = result
                result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.molar_entropy = result
                result = Me.AUX_CONDTG(T, P)
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.thermalConductivity = result
                result = Me.AUX_VAPVISCm(T, Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.GetValueOrDefault, Me.AUX_MMM(Phase))
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.viscosity = result
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.kinematic_viscosity = result / Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.Value

            ElseIf phaseID = 1 Then

                DW_CalcLiqMixtureProps()


            Else

                DW_CalcOverallProps()

            End If


            If phaseID > 0 Then
                result = overallmolarflow * phasemolarfrac * Me.AUX_MMM(Phase) / 1000 / Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.GetValueOrDefault
                Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.volumetric_flow = result
            Else
                'result = Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.massflow.GetValueOrDefault / Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.density.GetValueOrDefault
                'Me.CurrentMaterialStream.Phases(phaseID).SPMProperties.volumetric_flow = result
            End If


        End Sub

        Public Overrides Function DW_CalcPVAP_ISOL(ByVal T As Double) As Double
            Return Me.m_props.Pvp_leekesler(T, Me.RET_VTC(Phase.Liquid), Me.RET_VPC(Phase.Liquid), Me.RET_VW(Phase.Liquid))
        End Function

        Public Overrides Sub DW_CalcMassFlow()
            With Me.CurrentMaterialStream
                .Phases(0).SPMProperties.massflow = .Phases(0).SPMProperties.molarflow.GetValueOrDefault * Me.AUX_MMM(Phase.Mixture) / 1000
            End With
        End Sub

        Public Overrides Sub DW_CalcMolarFlow()
            With Me.CurrentMaterialStream
                .Phases(0).SPMProperties.molarflow = .Phases(0).SPMProperties.massflow.GetValueOrDefault / Me.AUX_MMM(Phase.Mixture) * 1000
            End With
        End Sub

        Public Overrides Sub DW_CalcVolumetricFlow()
            With Me.CurrentMaterialStream
                .Phases(0).SPMProperties.volumetric_flow = .Phases(0).SPMProperties.massflow.GetValueOrDefault / .Phases(0).SPMProperties.density.GetValueOrDefault
            End With
        End Sub

        Public Overrides Function DW_CalcDynamicViscosity_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double) As Double
            If Phase1 = Phase.Liquid Then
                Return Me.AUX_LIQVISCm(T)
            ElseIf Phase1 = Phase.Vapor Then
                Return Me.AUX_VAPVISCm(T, Me.AUX_VAPDENS(T, P), Me.AUX_MMM(Phase.Vapor))
            End If
        End Function

        Public Overrides Function SupportsComponent(ByVal comp As BaseThermoClasses.ConstantProperties) As Boolean

            Return True

        End Function

        Public Function RET_KIJ(ByVal id1 As String, ByVal id2 As String) As Double
            If Me.m_pr.InteractionParameters.ContainsKey(id1) Then
                If Me.m_pr.InteractionParameters(id1).ContainsKey(id2) Then
                    Return m_pr.InteractionParameters(id1)(id2).kij
                Else
                    If Me.m_pr.InteractionParameters.ContainsKey(id2) Then
                        If Me.m_pr.InteractionParameters(id2).ContainsKey(id1) Then
                            Return m_pr.InteractionParameters(id2)(id1).kij
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                End If
            Else
                Return 0
            End If
        End Function

        Public Overrides Function RET_VKij() As Double(,)

            Dim val(Me.CurrentMaterialStream.Phases(0).Components.Count - 1, Me.CurrentMaterialStream.Phases(0).Components.Count - 1) As Double
            Dim i As Integer = 0
            Dim l As Integer = 0

            i = 0
            For Each cp As Substance In Me.CurrentMaterialStream.Phases(0).Components.Values
                l = 0
                For Each cp2 As Substance In Me.CurrentMaterialStream.Phases(0).Components.Values
                    val(i, l) = Me.RET_KIJ(cp.Name, cp2.Name)
                    l = l + 1
                Next
                i = i + 1
            Next

            Return val

        End Function

#End Region

#Region "    Métodos Numéricos"

        Public Function IntegralSimpsonCp(ByVal a As Double,
                 ByVal b As Double,
                 ByVal Epsilon As Double, ByVal subst As String) As Double

            Dim Result As Double
            Dim switch As Boolean = False
            Dim h As Double
            Dim s As Double
            Dim s1 As Double
            Dim s2 As Double
            Dim s3 As Double
            Dim x As Double
            Dim tm As Double

            If a > b Then
                switch = True
                tm = a
                a = b
                b = tm
            ElseIf Abs(a - b) < 0.01 Then
                Return 0
            End If

            s2 = 1.0#
            h = b - a
            s = Me.AUX_CPi(subst, a) + Me.AUX_CPi(subst, b)
            Do
                s3 = s2
                h = h / 2.0#
                s1 = 0.0#
                x = a + h
                Do
                    s1 = s1 + 2.0# * Me.AUX_CPi(subst, x)
                    x = x + 2.0# * h
                Loop Until Not x < b
                s = s + s1
                s2 = (s + s1) * h / 3.0#
                x = Abs(s3 - s2) / 15.0#
            Loop Until Not x > Epsilon
            Result = s2

            If switch Then Result = -Result

            IntegralSimpsonCp = Result

        End Function

        Public Function IntegralSimpsonCp_T(ByVal a As Double,
         ByVal b As Double,
         ByVal Epsilon As Double, ByVal subst As String) As Double

            'Cp = A + B*T + C*T^2 + D*T^3 + E*T^4 where Cp in kJ/kg-mol , T in K 


            Dim Result As Double
            Dim h As Double
            Dim s As Double
            Dim s1 As Double
            Dim s2 As Double
            Dim s3 As Double
            Dim x As Double
            Dim tm As Double
            Dim switch As Boolean = False

            If a > b Then
                switch = True
                tm = a
                a = b
                b = tm
            ElseIf Abs(a - b) < 0.01 Then
                Return 0
            End If

            s2 = 1.0#
            h = b - a
            s = Me.AUX_CPi(subst, a) / a + Me.AUX_CPi(subst, b) / b
            Do
                s3 = s2
                h = h / 2.0#
                s1 = 0.0#
                x = a + h
                Do
                    s1 = s1 + 2.0# * Me.AUX_CPi(subst, x) / x
                    x = x + 2.0# * h
                Loop Until Not x < b
                s = s + s1
                s2 = (s + s1) * h / 3.0#
                x = Abs(s3 - s2) / 15.0#
            Loop Until Not x > Epsilon
            Result = s2

            If switch Then Result = -Result

            IntegralSimpsonCp_T = Result

        End Function

#End Region

        Public Overrides Function DW_CalcEnthalpy(ByVal Vx As Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double

            Dim H As Double

            If st = State.Liquid Then
                H = Me.m_pr.H_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Vx))
            Else
                H = Me.m_pr.H_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Vx))
            End If

            Return H

        End Function

        Public Overrides Function DW_CalcEnthalpyDeparture(ByVal Vx As Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double
            Dim H As Double

            If st = State.Liquid Then
                H = Me.m_pr.H_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            Else
                H = Me.m_pr.H_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            End If

            Return H

        End Function

        Public Overrides Function DW_CalcEntropy(ByVal Vx As Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double

            Dim S As Double

            If st = State.Liquid Then
                S = Me.m_pr.S_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Sid(298.15, T, P, Vx))
            Else
                S = Me.m_pr.S_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Sid(298.15, T, P, Vx))
            End If

            Return S

        End Function

        Public Overrides Function DW_CalcEntropyDeparture(ByVal Vx As Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double
            Dim S As Double

            If st = State.Liquid Then
                S = Me.m_pr.S_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            Else
                S = Me.m_pr.S_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            End If

            Return S

        End Function

        Public Overrides Function DW_CalcCv_ISOL(ByVal Phase1 As Phase, ByVal T As Double, ByVal P As Double) As Double
            Select Case Phase1
                Case Phase.Liquid
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Phase.Aqueous
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Phase.Liquid1
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Phase.Liquid2
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Phase.Liquid3
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Phase.Vapor
                    Return Me.m_props.CpCvR("V", T, P, RET_VMOL(Phase1), RET_VKij(), RET_VMAS(Phase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
            End Select
        End Function

        Public Overrides Sub DW_CalcCompPartialVolume(ByVal phase As Phase, ByVal T As Double, ByVal P As Double)

            Dim partvol As New Object
            Dim key As String = "0"
            Dim i As Integer = 0

            If Not Me.Parameters.ContainsKey("PP_USE_EOS_LIQDENS") Then Me.Parameters.Add("PP_USE_EOS_LIQDENS", 0)

            Select Case phase
                Case Phase.Liquid
                    key = "1"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Name, T)))
                        Next
                    End If
                Case Phase.Aqueous
                    key = "6"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Name, T)))
                        Next
                    End If
                Case Phase.Liquid1
                    key = "3"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Name, T)))
                        Next
                    End If
                Case Phase.Liquid2
                    key = "4"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Name, T)))
                        Next
                    End If
                Case Phase.Liquid3
                    key = "5"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Name, T)))
                        Next
                    End If
                Case Phase.Vapor
                    partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "V", 0.01)
                    key = "2"
            End Select

            i = 0
            For Each subst As Substance In Me.CurrentMaterialStream.Phases(key).Components.Values
                subst.PartialVolume = partvol(i)
                i += 1
            Next

        End Sub

        Public Overrides Function AUX_VAPDENS(ByVal T As Double, ByVal P As Double) As Double
            Dim val As Double
            Dim Z As Double = Me.m_pr.Z_PR(T, P, RET_VMOL(Phase.Vapor), RET_VKij, RET_VTC, RET_VPC, RET_VW, "V")
            val = P / (Z * 8.314 * T) / 1000 * AUX_MMM(Phase.Vapor)
            Return val
        End Function

        Public Overrides Function DW_CalcFugCoeff(ByVal Vx As Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double()

            Dim prn As New PropertyPackages.ThermoPlugs.PR

            Dim lnfug As Object

            If st = State.Liquid Then
                lnfug = prn.CalcLnFug(T, P, Vx, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, Nothing, "L")
            Else
                lnfug = prn.CalcLnFug(T, P, Vx, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, Nothing, "V")
            End If

            Dim n As Integer = UBound(lnfug)
            Dim i As Integer
            Dim fugcoeff(n) As Double

            For i = 0 To n
                fugcoeff(i) = Exp(lnfug(i))
            Next

            Return fugcoeff

        End Function

    End Class

End Namespace

