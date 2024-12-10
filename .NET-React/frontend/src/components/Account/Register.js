import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import Image from "react-bootstrap/Image";
import { useFormik } from "formik";
import * as yup from "yup";
import PasswordRequirements from "./PasswordRequirements";
import { useUI } from "../../context/UIContext";
import { useAccount } from "../../context/AccountContext";

const Register = () => {
    const { showRegister, setShowRegister, setShowLogin } = useUI();
    const { register, registerError, setRegisterError } = useAccount();

    const handleLoginClick = () => {
        setShowRegister(false);
        setShowLogin(true);
        setRegisterError(null);
        formik.resetForm();
    };

    const handleRegisterClick = ({email, password}) => {
        setRegisterError(null);
        register(email, password);
        formik.resetForm();
    };

    const handleCloseClick = () => {
        setShowRegister(false);
        setRegisterError(null);
        formik.resetForm();
    };

    // Creates frontend validation that matches the backend validation requirements
    // We have used the Bootstrap and Formik documentation to help us with this
    // https://react-bootstrap.netlify.app/docs/forms/validation
    // https://formik.org/docs/api/useFormik
    const schema = yup.object().shape({
        email: yup.string().email('Not a valid email').required('Email required'),

        password: yup.string().required('Password required')
            .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/, "Password doesn't meet requirements"),

        confirmPassword: yup.string().required('Confirm password required')
            .oneOf([yup.ref('password'), null], 'Passwords must match')
    });

    // Formik hook to manage form state and validation
    const formik = useFormik({
        initialValues: { email: "", password: "", confirmPassword: "" },
        validationSchema: schema,
        onSubmit: values => handleRegisterClick(values)
    });

    return (
        <Modal show={showRegister} onHide={handleCloseClick} className="dark text-center">
            <Modal.Header closeButton className="flex-column align-items-center">
                <Image src="Spotshare_Logo.png" fluid className="" style={{ width: '100px' }} />
                <Modal.Title>Register</Modal.Title>
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
                                setRegisterError(null);
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
                        <PasswordRequirements password={formik.values.password} />

                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Confirm Password</Form.Label>
                        <Form.Control
                            type="password"
                            name="confirmPassword"
                            placeholder="Confirm password"
                            value={formik.values.confirmPassword}
                            onChange={e => {
                                formik.handleChange(e);
                                formik.setFieldTouched('confirmPassword', true, false);
                            }}
                            isInvalid={formik.touched.confirmPassword && !!formik.errors.confirmPassword}
                            isValid={formik.touched.confirmPassword && !formik.errors.confirmPassword} />
                        <Form.Control.Feedback type="invalid">
                            {formik.errors.confirmPassword}
                        </Form.Control.Feedback>
                    </Form.Group>
                    {registerError && <p className="text-danger">{registerError}</p>}
                    <Form.Group>
                        <Button type="submit" className="w-100">Register</Button>
                    </Form.Group>
                </Form>
                <p>Already have an account? <Button variant="link" onClick={handleLoginClick}>Login</Button></p>
            </Modal.Body>
        </Modal>
    );
};

export default Register;