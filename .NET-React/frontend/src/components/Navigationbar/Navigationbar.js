import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';
import 'bootstrap-icons/font/bootstrap-icons.css';
import Form from 'react-bootstrap/Form';
import { useUI } from '../../context/UIContext';
import { useAccount } from '../../context/AccountContext';


const Navigationbar = () => {
    const { isDarkTheme, setIsDarkTheme, setShowSidebar, setShowLogin, setShowRegister, setShowAccount } = useUI();
    const { loggedIn, logOut } = useAccount();

    const handleThemeSwitch = () => {
        setIsDarkTheme(!isDarkTheme);
        document.body.setAttribute('data-bs-theme', isDarkTheme ? 'light':'dark');
    };

    const handleLoginClick = () => setShowLogin(true);
    const handleRegisterClick = () => setShowRegister(true);
    const handleAccountClick = () => setShowAccount(true);
    const handleSidebarButtonClick = () => setShowSidebar(true);

    return (
        <div>
            <Navbar expand className="bg-body-tertiary">
                <Container fluid>
                    <Button onClick={handleSidebarButtonClick} className="border-0 btn bg-transparent p-0">
                        <i className={`text-${isDarkTheme ? 'light':'dark'} bi bi-list fs-1`}></i>
                    </Button>
                    
                    <Navbar.Brand href="/">

                    Spotshare</Navbar.Brand>
                    <Nav>
                        {loggedIn
                        ? <><Nav.Link onClick={handleAccountClick}>Account</Nav.Link>
                            <Nav.Link onClick={logOut}>Logout</Nav.Link></>
                        : <><Nav.Link onClick={handleLoginClick}>Login</Nav.Link>
                            <Nav.Link onClick={handleRegisterClick}>Register</Nav.Link></>}
                            <Form.Check className="p-2 ps-5 border-0" type="switch" label={isDarkTheme ? 'dark':'light'} onClick={handleThemeSwitch}/>
                    </Nav>
                </Container>
            </Navbar>
        </div>
    )
};

export default Navigationbar;