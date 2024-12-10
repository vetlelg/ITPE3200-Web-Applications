import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import Image from "react-bootstrap/Image";
import * as yup from "yup";
import { useFormik } from "formik";
import { useUI } from "../../context/UIContext";
import { useAccount } from "../../context/AccountContext";

const Login = () => {
    const { showLogin, setShowLogin, setShowRegister } = useUI();
    const { logIn, loginError, setLoginError } = useAccount();

    const handleLoginClick = ({email, password, rememberMe}) => {
        setLoginError(null);
        logIn(email, password, rememberMe);
        formik.resetForm();
    }

    const handleRegisterClick = () => {
        setShowLogin(false);
        setShowRegister(true);
        setLoginError(null);
        formik.resetForm();
    };

    const handleCloseClick = () => {
        setShowLogin(false);
        setLoginError(null);
        formik.resetForm();
    };

    // Creates login input validation
    // We have used the Bootstrap and Formik documentation to help us with this
    // https://react-bootstrap.netlify.app/docs/forms/validation
    // https://formik.org/docs/api/useFormik
    const schema = yup.object().shape({
        email: yup.string().email('Not a valid email').required('Email required'),
        password: yup.string().required('Password required'),
        rememberMe: yup.boolean()
    });

    // Formik hook to manage form state and validation
    const formik = useFormik({
        initialValues: { email: "", password: "", rememberMe: false},
        validationSchema: schema,
        onSubmit: values => handleLoginClick(values),
    });
    
    return (
        <Modal show={showLogin} onHide={handleCloseClick} className="dark text-center">
            <Modal.Header closeButton className="flex-column align-items-center">
                <Image src="Spotshare_Logo.png" fluid className=""  style={{width: '100px'}}/>
                <Modal.Title>Login</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form noValidate onSubmit={formik.handleSubmit}>
                    <Form.Group className="mb-3">
                        <Form.Label>Email</Form.Label>
                        <Form.Control
                            type="email"
                            name="email"
                            placeholder="Enter email"
                            value={formik.values.email}
                            onChange={e => {
                                formik.handleChange(e);
                                formik.setFieldTouched('email', true, false);
                                setLoginError(null);
                            }}
                            isInvalid={formik.touched.email && !!formik.errors.email}
                            isValid={formik.touched.email && !formik.errors.email} />
                            <Form.Control.Feedback type="invalid">
                                {formik.errors.email}
                            </Form.Control.Feedback>
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Password</Form.Label>
                        <Form.Control
                            type="password"
                            name="password"
                            placeholder="Enter password"
                            value={formik.values.password}
                            onChange={e => {
                                formik.handleChange(e);
                                formik.setFieldTouched('password', true, false);
                            }}
                            isInvalid={formik.touched.password && !!formik.errors.password}
                            isValid={formik.touched.password && !formik.errors.password} />
                            <Form.Control.Feedback type="invalid">
                                {formik.errors.password}
                                
                            </Form.Control.Feedback>
                    </Form.Group>
                    <Form.Group className="mb-3 d-flex justify-content-center">
                        <Form.Check
                            type="checkbox"
                            name="rememberMe"
                            label="Remember me"
                            value={formik.values.rememberMe}
                            onChange={formik.handleChange}
                             />
                    </Form.Group>
                    
                    {loginError && <p className="text-danger">{loginError}</p>}
                    
                    <Form.Group>
                        <Button type="submit" className="w-100">Login</Button>
                    </Form.Group>
                </Form>
                <p>Dont have an account? <Button variant="link" onClick={handleRegisterClick}>Register</Button></p>
            </Modal.Body>
        </Modal>
    );
};

export default Login;