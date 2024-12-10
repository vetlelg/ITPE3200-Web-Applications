import React, { createContext, useState, useContext } from 'react';

/**
 * Context and custom hook for managing UI state for conditional rendering
 * State variables related to showing and hiding UI components
 * and closely related functions are defined here
 */

// Creates the context
const UIContext = createContext();

// The provider is used to wrap the components that need access to the context
// It's necessary to insert testValue as a parameter to the provider because
// the provider is used in the tests and the value object is passed as a parameter
export const UIProvider = ({ children, testValue }) => {
    // isDarkTheme is used to switch between dark and light theme
    // This is applied to the map and the UI components
    const [isDarkTheme, setIsDarkTheme] = useState(true);

    // These functions are used to show and hide the UI components
    const [showPointCreate, setShowPointCreate] = useState(false);
    const [showPointEdit, setShowPointEdit] = useState(false);
    const [showPoint, setShowPoint] = useState(false);
    const [showSidebar, setShowSidebar] = useState(false);
    const [showAccount, setShowAccount] = useState(false);
    const [showLogin, setShowLogin] = useState(false);
    const [showRegister, setShowRegister] = useState(false);
    const [showImageCarousel, setShowImageCarousel] = useState(false);

    // The value object contains the state variables and functions that are passed to the components
    const value = {
        showPointCreate,
        setShowPointCreate,
        showPointEdit,
        setShowPointEdit,
        showPoint,
        setShowPoint,
        showSidebar,
        setShowSidebar,
        showAccount,
        setShowAccount,
        showLogin,
        setShowLogin,
        showRegister,
        setShowRegister,
        isDarkTheme,
        setIsDarkTheme,
        showImageCarousel,
        setShowImageCarousel,
    };


    // The provider wraps the child components that need access to the context
    // If testValue is provided, it is used instead of the actual value
    return (
        <UIContext.Provider value={testValue || value}>
            {children}
        </UIContext.Provider>
    );
};

// Custom hook to access the context
export const useUI = () => useContext(UIContext);