﻿// GUI Animator FREE
// Version: 1.2.0
// Compatilble: Unity 2017.3.1 or higher, see more info in Readme.txt file.
//
// Developer:							Gold Experience Team (https://assetstore.unity.com/publishers/4162)
//
// Unity Asset Store:					https://assetstore.unity.com/packages/tools/gui/gui-animator-free-58843
// See Full version:					https://assetstore.unity.com/packages/tools/gui/gui-animator-for-unity-ui-28709
//
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

// ######################################################################
// GA_FREE_Demo07 class
// - Animates all GUIAnimFREE elements in the scene.
// - Responds to user mouse click or tap on buttons.
//
// Note this class is attached with "-SceneController-" object in "GA FREE - Demo07 (960x600px)" scene.
// ######################################################################

public class GA_FREE_Demo07 : MonoBehaviour
{

	// ########################################
	// Variables
	// ########################################
	
	#region Variables

	// Canvas
	public Canvas m_Canvas;
	
	// GUIAnimFREE objects of title text
	public GUIAnimFREE m_Title1;
	public GUIAnimFREE m_Title2;
	
	// GUIAnimFREE objects of top and bottom bars
	public GUIAnimFREE m_TopBar;
	public GUIAnimFREE m_BottomBar;
	
	// GUIAnimFREE object of dialogs
	public GUIAnimFREE m_Dialog;
	public GUIAnimFREE m_DialogButtons;
	
	// GUIAnimFREE objects of buttons
	public GUIAnimFREE m_Button1;
	public GUIAnimFREE m_Button2;
	public GUIAnimFREE m_Button3;
	public GUIAnimFREE m_Button4;
	
	#endregion // Variables
	
	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################
	
	#region MonoBehaviour
	
	// Awake is called when the script instance is being loaded.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
	void Awake ()
	{
		if(enabled)
		{
			// Set GUIAnimSystemFREE.Instance.m_AutoAnimation to false in Awake() will let you control all GUI Animator elements in the scene via scripts.
			GUIAnimSystemFREE.Instance.m_AutoAnimation = false;
		}
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
	void Start ()
	{
		// MoveIn m_TopBar and m_BottomBar
		m_TopBar.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_BottomBar.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// MoveIn m_Title1 and m_Title2
		StartCoroutine(MoveInTitleGameObjects());

		// Disable all scene switch buttons
		// http://docs.unity3d.com/Manual/script-GraphicRaycaster.html
		GUIAnimSystemFREE.Instance.SetGraphicRaycasterEnable(m_Canvas, false);
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
	void Update ()
	{		
	}
	
	#endregion // MonoBehaviour
	
	// ########################################
	// MoveIn/MoveOut functions
	// ########################################
	
	#region MoveIn/MoveOut
	
	// MoveIn m_Title1 and m_Title2
	IEnumerator MoveInTitleGameObjects()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveIn m_Title1 and m_Title2
		m_Title1.PlayInAnims(GUIAnimSystemFREE.eGUIMove.Self);
		m_Title2.PlayInAnims(GUIAnimSystemFREE.eGUIMove.Self);
		
		// MoveIn all dialogs and buttons
		StartCoroutine(MoveInPrimaryButtons());
	}
	
	// MoveIn all dialogs and buttons
	IEnumerator MoveInPrimaryButtons()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveIn all dialogs
		m_Dialog.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_DialogButtons.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// MoveIn all buttons
		m_Button1.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);	
		m_Button2.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);	
		m_Button3.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);	
		m_Button4.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// Enable all scene switch buttons
		StartCoroutine(EnableAllDemoButtons());
	}
	
	// MoveOut all dialogs and buttons
	public void HideAllGUIs()
	{
		// MoveOut all dialogs
		m_Dialog.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_DialogButtons.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// MoveOut all buttons
		m_Button1.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button2.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button3.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button4.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// MoveOut m_Title1 and m_Title2
		StartCoroutine(HideTitleTextMeshes());
	}
	
	// MoveOut m_Title1 and m_Title2
	IEnumerator HideTitleTextMeshes()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveOut m_Title1 and m_Title2
		m_Title1.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.Self);
		m_Title2.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.Self);
		
		// MoveOut m_TopBar and m_BottomBar
		m_TopBar.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_BottomBar.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
	}
	
	#endregion // MoveIn/MoveOut
	
	// ########################################
	// Enable/Disable button functions
	// ########################################
	
	#region Enable/Disable buttons
	
	// Enable/Disable all scene switch Coroutine
	IEnumerator EnableAllDemoButtons()
	{
		yield return new WaitForSeconds(1.0f);

		// Enable all scene switch buttons
		// http://docs.unity3d.com/Manual/script-GraphicRaycaster.html
		GUIAnimSystemFREE.Instance.SetGraphicRaycasterEnable(m_Canvas, true);
	}

	// Disable all buttons for a few seconds
	IEnumerator DisableButtonForSeconds(GameObject GO, float DisableTime)
	{
		// Disable all buttons
		GUIAnimSystemFREE.Instance.EnableButton(GO.transform, false);
		
		yield return new WaitForSeconds(DisableTime);
		
		// Enable all buttons
		GUIAnimSystemFREE.Instance.EnableButton(GO.transform, true);
	}
	
	#endregion // Enable/Disable buttons
	
	// ########################################
	// UI Responder functions
	// ########################################
	
	#region UI Responder

	public void OnButton_1()
	{
		// MoveOut m_Button1
		MoveButtonsOut();
		
		// Disable m_Button1, m_Button2, m_Button3, m_Button4 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Button1.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button2.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button3.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button4.gameObject, 2.0f));

		// Set next move in of m_Button1 to new position
		StartCoroutine(SetButtonMove(GUIAnimFREE.ePosMove.UpperScreenEdge, GUIAnimFREE.ePosMove.UpperScreenEdge));
	}
	
	public void OnButton_2()
	{
		// MoveOut m_Button2
		MoveButtonsOut();
		
		// Disable m_Button1, m_Button2, m_Button3, m_Button4 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Button1.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button2.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button3.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button4.gameObject, 2.0f));
		
		// Set next move in of m_Button2 to new position
		StartCoroutine(SetButtonMove(GUIAnimFREE.ePosMove.LeftScreenEdge, GUIAnimFREE.ePosMove.LeftScreenEdge));
	}
	
	public void OnButton_3()
	{
		// MoveOut m_Button3
		MoveButtonsOut();
		
		// Disable m_Button1, m_Button2, m_Button3, m_Button4 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Button1.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button2.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button3.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button4.gameObject, 2.0f));

		// Set next move in of m_Button3 to new position
		StartCoroutine(SetButtonMove(GUIAnimFREE.ePosMove.RightScreenEdge, GUIAnimFREE.ePosMove.RightScreenEdge));
	}
	
	public void OnButton_4()
	{
		// MoveOut m_Button4
		MoveButtonsOut();
		
		// Disable m_Button1, m_Button2, m_Button3, m_Button4 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Button1.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button2.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button3.gameObject, 2.0f));
		StartCoroutine(DisableButtonForSeconds(m_Button4.gameObject, 2.0f));
		
		// Set next move in of m_Button3 to new position
		StartCoroutine(SetButtonMove(GUIAnimFREE.ePosMove.BottomScreenEdge, GUIAnimFREE.ePosMove.BottomScreenEdge));
	}
	
	public void OnDialogButton()
	{
		// MoveOut m_Dialog
		m_Dialog.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_DialogButtons.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// Disable m_DialogButtons for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_DialogButtons.gameObject, 2.0f));

		// Moves m_Dialog back to screen
		StartCoroutine(DialogMoveIn());
	}
	
	#endregion // UI Responder
	
	// ########################################
	// Move Dialog/Button functions
	// ########################################
	
	#region Move Dialog/Button
	
	// MoveOut all buttons
	void MoveButtonsOut()
	{
		m_Button1.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button2.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button3.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_Button4.PlayOutAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
	}
	
	// Set next move in of all buttons to new position
	IEnumerator SetButtonMove(GUIAnimFREE.ePosMove PosMoveIn, GUIAnimFREE.ePosMove PosMoveOut)
	{
		yield return new WaitForSeconds(2.0f);
		
		// Set next MoveIn position of m_Button1 to PosMoveIn
		m_Button1.m_MoveIn.MoveFrom = PosMoveIn;
		// Reset m_Button1
		m_Button1.Reset();
		// MoveIn m_Button1
		m_Button1.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// Set next MoveIn position of m_Button2 to PosMoveIn
		m_Button2.m_MoveIn.MoveFrom = PosMoveIn;
		// Reset m_Button2
		m_Button2.Reset();
		// MoveIn m_Button2
		m_Button2.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// Set next MoveIn position of m_Button3 to PosMoveIn
		m_Button3.m_MoveIn.MoveFrom = PosMoveIn;
		// Reset m_Button3
		m_Button3.Reset();
		// MoveIn m_Button3
		m_Button3.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		
		// Set next MoveIn position of m_Button4 to PosMoveIn
		m_Button4.m_MoveIn.MoveFrom = PosMoveIn;
		// Reset m_Button4
		m_Button4.Reset();
		// MoveIn m_Button4
		m_Button4.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
	}
	
	// Moves m_Dialog back to screen
	IEnumerator DialogMoveIn()
	{
		yield return new WaitForSeconds(1.5f);
		
		m_Dialog.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
		m_DialogButtons.PlayInAnims(GUIAnimSystemFREE.eGUIMove.SelfAndChildren);
	}
	
	#endregion // Move Dialog/Button
}
