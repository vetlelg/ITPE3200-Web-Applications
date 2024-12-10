import { UIProvider } from "./UIContext";
import { AccountProvider } from "./AccountContext";
import { PointProvider } from "./PointContext";

/**
 * Combines all providers into one
 * The purpose of the providers is to make the context available to components
 * Components need to be wrapped in this provider to access the context
 * The custom hooks useUI, useAccount and usePoint are used to access the context
 */

const AppProvider = ({ children }) => {
    return (
        <UIProvider>
            <AccountProvider>
                <PointProvider>
                    {children}
                </PointProvider>
            </AccountProvider>
        </UIProvider>
    );
};

export default AppProvider;