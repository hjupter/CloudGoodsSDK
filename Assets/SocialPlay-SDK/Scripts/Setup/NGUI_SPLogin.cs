﻿using UnityEngine;
using System.Collections;
using System;

public class NGUI_SPLogin : MonoBehaviour
{


    #region Login variables
    public GameObject loginTab;
    public UIInput loginUserEmail;
    public UIInput loginUserPassword;
    public UILabel loginErrorLabel;

    public GameObject resendVerificationTextObject;

    private UIInputVisualValidation loginUserEmailValidator;
    private UIInputVisualValidation loginUserPasswordValidator;

    #endregion

    #region Register variables
    public GameObject registerTab;
    public UIInput registerUserEmail;
    public UIInput registerUserPassword;
    public UIInput registerUserPasswordConfirm;
    public UIInput registerUserName;
    public UILabel registerErrorLabel;


    private UIInputVisualValidation registerUserEmailValidator;
    private UIInputVisualValidation registerUserPasswordValidator;
    private UIInputVisualValidation registerUserPasswordConfirmValidator;
    #endregion

    #region Register Confirm Variables
    public GameObject registerConfirmationTab;
    public UILabel RegisterConfirmationStatus;
    public UIButton registerConfirmButton;

    #endregion

    void OnEnable()
    {
        SPLogin.loginMessageResponce += RecivedLoginResponce;
        SPLogin.recivedUserInfo += RecivedUserGuid;
        SPLogin.RegisterMessageResponce += RegisterMessageResponce;
        SPLogin.ForgotPasswordResponce += ForgotPasswordResponce;
        SPLogin.ResentVerificationResponce += ResentVerificationResponce;
    }

    void OnDisable()
    {
        SPLogin.loginMessageResponce -= RecivedLoginResponce;
        SPLogin.recivedUserInfo -= RecivedUserGuid;
        SPLogin.RegisterMessageResponce -= RegisterMessageResponce;
        SPLogin.ForgotPasswordResponce -= ForgotPasswordResponce;
        SPLogin.ResentVerificationResponce -= ResentVerificationResponce;
    }

    void Start()
    {
        loginTab.SetActive(true);
        registerErrorLabel.text = "";
        registerTab.SetActive(false);
        registerConfirmationTab.SetActive(false);

        loginUserEmailValidator = loginUserEmail.GetComponent<UIInputVisualValidation>();
        loginUserPasswordValidator = loginUserPassword.GetComponent<UIInputVisualValidation>();


        registerUserEmailValidator = registerUserEmail.GetComponent<UIInputVisualValidation>();
        registerUserPasswordValidator = registerUserPassword.GetComponent<UIInputVisualValidation>(); ;
        registerUserPasswordConfirmValidator = registerUserPasswordConfirm.GetComponent<UIInputVisualValidation>();
        resendVerificationTextObject.SetActive(false);

    }

    #region webservice responce events

    void RecivedUserGuid(SPLogin.UserInfo obj)
    {
        resendVerificationTextObject.SetActive(false);
        loginErrorLabel.text = "User logged in";
        this.gameObject.SetActive(false);
    }

    void ResentVerificationResponce(SPLogin.SPLogin_Responce responce)
    {
        resendVerificationTextObject.SetActive(false);
        loginErrorLabel.text = responce.message;
    }

    void ForgotPasswordResponce(SPLogin.SPLogin_Responce responce)
    {
        resendVerificationTextObject.SetActive(false);
        loginErrorLabel.text = responce.message;
    }

    void RecivedLoginResponce(SPLogin.SPLogin_Responce recivedMessage)
    {
        if (recivedMessage.code == 3)
        {
            resendVerificationTextObject.SetActive(true);
            return;
        }

        resendVerificationTextObject.SetActive(false);
        loginErrorLabel.text = recivedMessage.message;
    }

    void RegisterMessageResponce(SPLogin.SPLogin_Responce responce)
    {
        resendVerificationTextObject.SetActive(false);
   
        if (responce.code == 0)
        {
            RegisterConfirmationStatus.text = "Verification Email has been sent to your Email";
            registerConfirmButton.onClick.Clear();
            registerConfirmButton.onClick.Add(new EventDelegate(this, "SwitchToLogin"));
            registerConfirmButton.GetComponentInChildren<UILabel>().text = "To Login";
        }
        else
        {            
            RegisterConfirmationStatus.text = responce.message;
            registerConfirmButton.onClick.Clear();
            registerConfirmButton.onClick.Add(new EventDelegate(this, "SwitchToRegister"));
            registerConfirmButton.GetComponentInChildren<UILabel>().text = "Back";
        }
    }

    void LoginSuccess(Guid userID)
    {
        loginErrorLabel.text = userID.ToString();
    }
    #endregion

    #region button functions

    public void SwitchToRegister()
    {
        registerErrorLabel.text = "";
        loginTab.SetActive(false);
        registerTab.SetActive(true);
        registerConfirmationTab.SetActive(false);
    }

    public void SwitchToLogin()
    {
        loginErrorLabel.text = "";
        registerTab.SetActive(false);
        loginTab.SetActive(true);
        registerConfirmationTab.SetActive(false);
    }

    public void Login()
    {
        string ErrorMsg = "";
        if (!loginUserEmailValidator.IsValidCheck())
        {
            ErrorMsg = "-Invalid Email";
        }

        if (!loginUserPasswordValidator.IsValidCheck())
        {
            if (!string.IsNullOrEmpty(ErrorMsg)) ErrorMsg += "\n";
            ErrorMsg += "-Invalid Password";
        }
        loginErrorLabel.text = ErrorMsg;
        if (string.IsNullOrEmpty(ErrorMsg))
        {
            SPLogin.Login(loginUserEmail.value, loginUserPassword.value);
        }
    }

    public void Register()
    {

        string ErrorMsg = "";
        if (!registerUserEmailValidator.IsValidCheck())
        {
            ErrorMsg = "-Invalid Email";
        }

        if (!registerUserPasswordValidator.IsValidCheck() || !registerUserPasswordConfirmValidator.IsValidCheck())
        {
            if (!string.IsNullOrEmpty(ErrorMsg)) ErrorMsg += "\n";
            ErrorMsg += "-Invalid Password";
        }
        registerErrorLabel.text = ErrorMsg;
        if (string.IsNullOrEmpty(ErrorMsg))
        {
            registerConfirmationTab.SetActive(true);
            registerTab.SetActive(false);
            SPLogin.RegisterUser(registerUserEmail.value, registerUserPassword.value, registerUserName.value);
        }
    }

    public void ForgotPassword()
    {
        string ErrorMsg = "";
        if (!loginUserEmailValidator.IsValidCheck())
        {
            ErrorMsg = "Password reset requires valid E-mail";
        }
        loginErrorLabel.text = ErrorMsg;
        SPLogin.ForgotPassword(loginUserEmail.value);
    }

    public void ResendVerificationEmail()
    {
        string ErrorMsg = "";
        if (!loginUserEmailValidator.IsValidCheck())
        {
            ErrorMsg = "Validation resend requires valid E-mail";
        }
        loginErrorLabel.text = ErrorMsg;
        SPLogin.ResendVerificationEmail(loginUserEmail.value);

    }

    #endregion
}
