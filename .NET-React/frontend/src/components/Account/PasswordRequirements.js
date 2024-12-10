// Simple component to display password requirements in the registration form
const PasswordRequirements = ({ password }) => {
    const requirements = [
        { regex: /[a-z]/, label: "At least one lowercase letter" },
        { regex: /[A-Z]/, label: "At least one uppercase letter" },
        { regex: /\d/, label: "At least one number" },
        { regex: /.{8,}/, label: "At least 8 characters long" }
    ];

    return (
        <ul className="text-start mt-2">
            {requirements.map((req, index) => (
                <li key={index} className={req.regex.test(password) ? 'text-success' : 'text-danger'} >
                    {req.label}
                </li>
            ))}
        </ul>
    );
};

export default PasswordRequirements;