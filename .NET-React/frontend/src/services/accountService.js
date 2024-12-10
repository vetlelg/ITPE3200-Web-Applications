import API_URL from './apiConfig';

const headers = {
    'Content-Type': 'application/json',
}

const handleResponse = async response => {
    if (response.ok) {
        if (response.status === 204)
            return null;
        return await response.json();
    } else {
        const error = await response.json();
        throw new Error(error.message);
    }
}

export const createAccount = async formData => {
    const response = await fetch(`${API_URL}/api/accounts`, {
        method: 'POST',
        headers,
        body: JSON.stringify(formData),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const changeEmail = async (accountId, email) => {
    const response = await fetch(`${API_URL}/api/accounts/${accountId}/email`, {
        method: 'PUT',
        headers,
        body: JSON.stringify({ email }),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const changePassword = async (accountId, password, oldPassword) => {
    const response = await fetch(`${API_URL}/api/accounts/${accountId}/password`, {
        method: 'PUT',
        headers,
        body: JSON.stringify({ password, oldPassword }),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const login = async formData => {
    const response = await fetch(`${API_URL}/api/accounts/login`, {
        method: 'POST',
        headers,
        body: JSON.stringify(formData),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const logout = async () => {
    const response = await fetch(`${API_URL}/api/accounts/logout`, {
        method: 'POST',
        headers,
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const checkAuth = async () => {
    const response = await fetch(`${API_URL}/api/accounts/checkAuth`, {
        method: 'GET',
        headers,
        credentials: 'include',
    });
    return await handleResponse(response);
}

