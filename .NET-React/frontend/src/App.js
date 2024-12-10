import HomePage from './components/Home/HomePage';
import AppProvider from './context/AppProvider';

const App = () => {
    return (
    <AppProvider>
        <HomePage />
    </AppProvider>
    );
};

export default App;
