import React, { createContext, useState, useEffect, useContext } from 'react';
import * as AccountService from '../services/accountService';
import { useUI } from './UIContext';

/**
 * Context and custom hook for managing accounts and authentication
 * State variables related to accounts and authentication
 * and closely related functions are defined here
 */

// Creates the context
const AccountContext = createContext();

// The provider is used to wrap the components that need access to the context
// It's necessary to insert testValue as a parameter to the provider because
// the provider is used in the tests and the value object is passed as a parameter
export const AccountProvider = ({ children, testValue }) => {
    // loggedIn and accountId are used to manage authentication and authorization
    // and conditionally render components based on the user's login status
    const [loggedIn, setLoggedIn] = useState(false);
    const [accountId, setAccountId] = useState(null);
    const [accountName, setAccountName] = useState(null);

    const [loginError, setLoginError] = useState(null);
    const [registerError, setRegisterError] = useState(null);
    const [changeEmailError, setChangeEmailError] = useState(null);
    const [changePasswordError, setChangePasswordError] = useState(null);
    const [changeEmailSuccess, setChangeEmailSuccess] = useState(null);
    const [changePasswordSuccess, setChangePasswordSuccess] = useState(null);

    const { setShowLogin, setShowRegister } = useUI();

    useEffect(() => {
        checkAuth();
    }, []);

    const checkAuth = async () => {
        try {
            const { isAuthenticated, accountId, email } = await AccountService.checkAuth();
            if (isAuthenticated) {
                setLoggedIn(true);
                setAccountId(accountId);
                setAccountName(email);
            }
        } catch (error) {
            console.error(`Authentication failed: ${error.message}`);
        }
    }

    const logIn = async (email, password, rememberMe) => {
        try {
            const response = await AccountService.login({ email, password, rememberMe });
            setLoggedIn(true);
            setAccountId(response.accountId);
            setAccountName(email);
            setShowLogin(false);
        } catch (error) {
            console.error(`Login failed: ${error.message}`);
            setLoginError(error.message);
        }
    };

    // handleLogout is used in the Navbar component to log out the user
    const logOut = async () => {
        try {
            await AccountService.logout();
            setLoggedIn(false);
            setAccountName(null);
            setAccountId(null);
        } catch (error) {
            console.error(`Logout failed: ${error.message}`);
        }
    }

    // Gets data from the register form and creates a new account in the database
    // This is used in the Register component
    const register = async (email, password) => {
        try {
            const { accountId } = await AccountService.createAccount({ email, password });
            setAccountName(email);
            setAccountId(accountId);
            setLoggedIn(true);
            setShowRegister(false);
        } catch (error) {
            console.error(`Error when registering: ${error.message}`);
            setRegisterError(error.message);
        }
    };
    // Changes the email of an account. This is used in the Account component
    const changeEmail = async email => {
        try {
            const response = await AccountService.changeEmail(accountId, email);
            setAccountName(email);
            setChangeEmailSuccess(response.message);
        } catch (error) {
            console.error(`Error when updating email: ${error.message}`);
            setChangeEmailError(error.message);
        }
    }
    // Changes the password of an account. This is used in the Account component
    const changePassword = async (password, oldPassword) => {
        try {
            const response = await AccountService.changePassword(accountId, password, oldPassword);
            setChangePasswordSuccess(response.message);
        } catch (error) {
            console.error(`Error when updating password: ${error.message}`);
            setChangePasswordError(error.message);
        }
    }

    // The value object contains the state variables and functions that are available to components
    const value = {
        loggedIn,
        accountId,
        accountName,
        loginError,
        registerError,
        changeEmailError,
        changePasswordError,
        changeEmailSuccess,
        changePasswordSuccess,
        setLoginError,
        setRegisterError,
        setChangeEmailError,
        setChangePasswordError,
        setChangeEmailSuccess,
        setChangePasswordSuccess,
        logIn,
        logOut,
        register,
        changeEmail,
        changePassword,
    }

    // The provider wraps the child components that need access to the context
    // If testValue is provided, it is used instead of the actual value
    return (
        <AccountContext.Provider value={testValue || value}>
            {children}
        </AccountContext.Provider>
    );
};

// Defines custom hook that allows components to access the context
export const useAccount = () => useContext(AccountContext);