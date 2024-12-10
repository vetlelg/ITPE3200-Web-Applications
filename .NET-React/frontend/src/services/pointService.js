import API_URL from './apiConfig';

const headers = {
    'Content-Type': 'application/json',
}

// HandleResponse
// Author: Baifan Zhou
// Source: Demo-React-8-ItemService.pdf - https://oslomet.instructure.com/courses/29069/pages/demo-react-8-itemservice?module_item_id=701661
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

export const getPoints = async () => {
    const response = await fetch(`${API_URL}/api/points`, {
        method: 'GET',
        headers,
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const getPointById = async (id) => {
    const response = await fetch(`${API_URL}/api/points/${id}`);
    return await handleResponse(response);
}

export const createPoint = async (point) => {
    point.pointId = 0;
    const response = await fetch(`${API_URL}/api/points/create`, {
        method: 'POST',
        headers,
        body: JSON.stringify(point),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const updatePoint = async (point) => {
    const response = await fetch(`${API_URL}/api/points/${point.pointId}`, {
        method: 'PUT',
        headers,
        body: JSON.stringify(point),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const deletePoint = async (id) => {
    const response = await fetch(`${API_URL}/api/points/${id}/delete`, {
        method: 'DELETE',
        credentials: 'include',
    });

    return await handleResponse(response);
}
export const createComment = async (comment) => {
    comment.commentId = 0;

    const response = await fetch(`${API_URL}/api/points/createComment`, {
        method: 'Post',
        headers,
        body: JSON.stringify(comment),
        credentials: 'include',
    });

    return await handleResponse(response);
}
export const deleteComment = async (pointId, commentId) => {
    let id = {pointId: pointId, commentId: commentId}
    const response = await fetch(`${API_URL}/api/points/deleteComment`, {
        method: 'POST',
        headers,
        body: JSON.stringify(id),
        credentials: 'include',
    });
    return await handleResponse(response);
}

export const deleteImage = async ( imageId ) => {
    const response = await fetch(`${API_URL}/api/points/deleteImage`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ imageId }),
        credentials: 'include',
    });
    return await handleResponse(response);
}