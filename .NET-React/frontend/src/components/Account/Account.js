import Modal from "react-bootstrap/Modal";
import Tab from "react-bootstrap/Tab";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import Nav from "react-bootstrap/Nav";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import * as yup from "yup";
import { useFormik } from "formik";
import { useUI } from "../../context/UIContext";
import { useAccount } from "../../context/AccountContext";
import { usePoint } from "../../context/PointContext";

const Account = () => {
    const { showAccount, setShowAccount } = useUI();
    const {
        accountName, changePassword, changeEmail, changePasswordError,
        setChangePasswordError, changeEmailError, setChangeEmailError,
        changePasswordSuccess, setChangePasswordSuccess, changeEmailSuccess, setChangeEmailSuccess
    } = useAccount();

    const { fetchPoints } = usePoint();

    const handleCloseClick = () => {
        setShowAccount(false);
        setChangePasswordError(null);
        setChangeEmailError(null);
        setChangePasswordSuccess(null);
        setChangeEmailSuccess(null);
        passwordFormik.resetForm();
        emailFormik.resetForm();
    };

    const handleChangePasswordClick = ({password, oldPassword}) => {
        changePassword(password, oldPassword);
        passwordFormik.resetForm();
    }

    const handleChangeEmailClick = async ({email}) => {
        await changeEmail(email);
        emailFormik.resetForm();
        // Update the email of all points and comments for the points created by the user
        await fetchPoints();
    }

    // Creates frontend validation that matches the backend validation requirements
    // We use Bootstrap and Formik to do this
    const passwordSchema = yup.object().shape({
        oldPassword: yup.string().required('Old password required'),
        password: yup.string().required('Password required')
            .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/, "Password doesn't meet requirements"),
        confirmPassword: yup.string().required('Confirm password required')
            .oneOf([yup.ref('password'), null], 'Passwords must match')
    });
    const emailSchema = yup.object().shape({
        email: yup.string().email('Not a valid email').required('Email required'),
    });

    // Formik hook to manage form state and validation
    const passwordFormik = useFormik({
        initialValues: { oldPassword: "", password: "", confirmPassword: "" },
        validationSchema: passwordSchema,
        onSubmit: values => handleChangePasswordClick(values)
    });

    const emailFormik = useFormik({
        initialValues: { email: "" },
        validationSchema: emailSchema,
        onSubmit: values => handleChangeEmailClick(values)
    });

    return (
        <Modal show={showAccount} onHide={handleCloseClick}>
            <Modal.Header closeButton>
                <Modal.Title>Manage Account</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Tab.Container defaultActiveKey="profile">
                    <Row>
                        <Col>
                            <Nav variant="pills" className="flex-column">
                                <Nav.Item>
                                    <Nav.Link eventKey="profile">Profile</Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link eventKey="email">Email</Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link eventKey="password">Password</Nav.Link>
                                </Nav.Item>
                            </Nav>
                        </Col>
                        <Col>
                            <Tab.Content>
                                <Tab.Pane eventKey="profile">
                                    <h1>Profile</h1>
                                    <p>Email: {accountName}</p>
                                </Tab.Pane>
                                <Tab.Pane eventKey="email">
                                    <h1>Change Email</h1>
                                    <Form noValidate onSubmit={emailFormik.handleSubmit}>
                                        <Form.Group className="mb-2">
                                            <Form.Control
                                                type="email"
                                                name="email"
                                                placeholder="Enter email"
                                                value={emailFormik.values.email}
                                                onChange={e => {
                                                    emailFormik.handleChange(e);
                                                    emailFormik.setFieldTouched('email', true, false);
                                                    setChangeEmailError(null);
                                                    setChangeEmailSuccess(null);
                                                }}
                                                isInvalid={emailFormik.touched.email && !!emailFormik.errors.email}
                                                isValid={emailFormik.touched.email && !emailFormik.errors.email} />
                                            <Form.Control.Feedback type="invalid">
                                                {emailFormik.errors.email}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                        {changeEmailError && <p className="text-danger">{changeEmailError}</p>}
                                        {changeEmailSuccess && <p className="text-success">{changeEmailSuccess}</p>}
                                        <Form.Group>
                                            <Button type="submit">Change Email</Button>
                                        </Form.Group>
                                    </Form>
                                </Tab.Pane>
                                <Tab.Pane eventKey="password">
                                    <h1>Change Password</h1>
                                    <Form noValidate onSubmit={passwordFormik.handleSubmit}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Old Password</Form.Label>
                                            <Form.Control
                                                type="password"
                                                name="oldPassword"
                                                placeholder="Enter old password"
                                                value={passwordFormik.values.oldPassword}
                                                onChange={e => {
                                                    passwordFormik.handleChange(e);
                                                    passwordFormik.setFieldTouched('oldPassword', true, false);
                                                    setChangePasswordError(null);
                                                    setChangePasswordSuccess(null);
                                                }}
                                                isInvalid={passwordFormik.touched.oldPassword && !!passwordFormik.errors.oldPassword}
                                                isValid={passwordFormik.touched.oldPassword && !passwordFormik.errors.oldPassword} />
                                            <Form.Control.Feedback type="invalid">
                                                {passwordFormik.errors.oldPassword}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Password</Form.Label>
                                            <Form.Control
                                                type="password"
                                                name="password"
                                                placeholder="Enter password"
                                                value={passwordFormik.values.password}
                                                onChange={e => {
                                                    passwordFormik.handleChange(e);
                                                    passwordFormik.setFieldTouched('password', true, false);
                                                    setChangePasswordError(null);
                                                    setChangePasswordSuccess(null);
                                                }}
                                                isInvalid={passwordFormik.touched.password && !!passwordFormik.errors.password}
                                                isValid={passwordFormik.touched.password && !passwordFormik.errors.password} />
                                            <Form.Control.Feedback type="invalid">
                                                {passwordFormik.errors.password}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Confirm Password</Form.Label>
                                            <Form.Control
                                                type="password"
                                                name="confirmPassword"
                                                placeholder="Confirm password"
                                                value={passwordFormik.values.confirmPassword}
                                                onChange={e => {
                                                    passwordFormik.handleChange(e);
                                                    passwordFormik.setFieldTouched('confirmPassword', true, false);
                                                    setChangePasswordError(null);
                                                    setChangePasswordSuccess(null);
                                                }}
                                                isInvalid={passwordFormik.touched.confirmPassword && !!passwordFormik.errors.confirmPassword}
                                                isValid={passwordFormik.touched.confirmPassword && !passwordFormik.errors.confirmPassword} />
                                            <Form.Control.Feedback type="invalid">
                                                {passwordFormik.errors.confirmPassword}
                                            </Form.Control.Feedback>
                                        </Form.Group>
                                        {changePasswordError && <p className="text-danger" >{changePasswordError}</p>}
                                        {changePasswordSuccess && <p className="text-success" >{changePasswordSuccess}</p>}
                                        <Form.Group>
                                            <Button type="submit">Change Password</Button>
                                        </Form.Group>
                                    </Form>
                                </Tab.Pane>
                            </Tab.Content>
                        </Col>
                    </Row>
                </Tab.Container>
            </Modal.Body>
        </Modal>

    )
}

export default Account;