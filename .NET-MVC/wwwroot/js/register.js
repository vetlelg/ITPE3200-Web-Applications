document.addEventListener("DOMContentLoaded", function () {
    const passwordInput = document.getElementById("Password");
    const confirmPasswordInput = document.getElementById("ConfirmPassword");
    const emailInput = document.getElementById("Email")

    emailInput.addEventListener("input", function() {
        const emailValue = emailInput.value;

        if (/^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$/.test(emailValue)) {
            emailInput.classList.remove("is-invalid");
            emailInput.classList.add("is-valid");
        } else {
            emailInput.classList.remove("is-valid");
            emailInput.classList.add("is-invalid");
        }
    });

    const requirements = {
        lower: document.getElementById("require-lower"),
        upper: document.getElementById("require-upper"),
        number: document.getElementById("require-number"),
        length: document.getElementById("require-length")
    };

    passwordInput.addEventListener("input", function () {
        const value = passwordInput.value;

        // Check if there is at least one lower case letter
        if (/[a-z]/.test(value)) {
            requirements.lower.classList.remove("invalid");
            requirements.lower.classList.add("valid");
        } else {
            requirements.lower.classList.remove("valid");
            requirements.lower.classList.add("invalid");
        }

        // Check if there is at least one upper case letter
        if (/[A-Z]/.test(value)) {
            requirements.upper.classList.remove("invalid");
            requirements.upper.classList.add("valid");
        } else {
            requirements.upper.classList.remove("valid");
            requirements.upper.classList.add("invalid");
        }

        // Check if there is at least one number
        if (/\d/.test(value)) {
            requirements.number.classList.remove("invalid");
            requirements.number.classList.add("valid");
        } else {
            requirements.number.classList.remove("valid");
            requirements.number.classList.add("invalid");
        }

        // Check that the password is at least 8 characters long
        if (value.length >= 8) {
            requirements.length.classList.remove("invalid");
            requirements.length.classList.add("valid");
        } else {
            requirements.length.classList.remove("valid");
            requirements.length.classList.add("invalid");
        }

        if (/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(value)) {
            passwordInput.classList.remove("is-invalid");
            passwordInput.classList.add("is-valid");
        } else {
            passwordInput.classList.remove("is-valid");
            passwordInput.classList.add("is-invalid");
        }
    });

    confirmPasswordInput.addEventListener("input", function() {
        let confirmPasswordValue = confirmPasswordInput.value;
        let passwordInputValue = passwordInput.value;
        
        if (confirmPasswordValue != passwordInputValue || confirmPasswordValue == "") {
            confirmPasswordInput.classList.remove("is-valid");
            confirmPasswordInput.classList.add("is-invalid");
        } else {
            confirmPasswordInput.classList.remove("is-invalid");
            confirmPasswordInput.classList.add("is-valid");
        }
    });
});