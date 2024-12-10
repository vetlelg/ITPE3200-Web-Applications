import React, { createContext, useState, useEffect, useContext } from 'react';
import * as PointService from '../services/pointService';

/**
 * Context and custom hook for managing points
 * State variables related to points
 * and closely related functions are defined here
 */

// Creates the context
const PointContext = createContext();

// The provider is used to wrap the components that need access to the context
// It's necessary to insert testValue as a parameter to the provider because
// the provider is used in the tests and the value object is passed as a parameter
export const PointProvider = ({ children, testValue }) => {
    // points holds all the points from the database
    const [points, setPoints] = useState([]);

    // For use in the search functionality in the sidebar
    // Ensures that only the matching points appear in both the pointlist and the map
    const [filteredPoints, setFilteredPoints] = useState([]);

    // selectedPoint is used to show the details of a point in the Point details component
    const [selectedPoint, setSelectedPoint] = useState(null);

    // used in the Edit-component for handling uploading of new images without affecting the
    // existing images
    const [uploadedImages, setUploadedImages] = useState([]);

    // latitude and longitude is set on map click
    const [latitude, setLatitude] = useState(null);
    const [longitude, setLongitude] = useState(null);

    useEffect(() => {
        fetchPoints();
    }, []);

    // Fetches points from the database and sets the points state
    // Sets error and loading state based on the result
    const fetchPoints = async () => {
        try {
            const points = await PointService.getPoints();
            setPoints(points);
        } catch (error) {
            console.error(`Error when fetching: ${error.message}`);
            return error.message;
        }
    };

    // Creates point in database
    const createPoint = async point => {
        try {
            const returnPoint = await PointService.createPoint(point);
            setPoints(prevPoints => [...prevPoints, returnPoint]);
        }
        catch (error) {
            console.error(`Error creating point: ${error.message}`);
            return error.message;
        }
    }

    // Edits point in database
    const editPoint = async point => {
        try {
            const updatedPoint = await PointService.updatePoint(point);
            setPoints(points.map(point =>
                point.pointId === updatedPoint.pointId ? { ...updatedPoint } : point));

            // So that points gets updated with the correct information in the map
            //const newFilteredPoints = filterPoints(newPoints, selectedUsers, query);
            //setFilteredPoints(newFilteredPoints);
            //setPoints(prevPoints => prevPoints.map(p => p.pointId === point.pointId ? { ...point } : p));
        } catch (error) {
            console.error(`Error creating point: ${error.message}`);
            return error.message;
        }
    }

    // Deletes point from database
    const deletePoint = async pointId => {
        try {
            await PointService.deletePoint(pointId);

            //Deletes selected point from the points list
            //Also need to update both points and filteredPoints to ensure that the map
            //shows the correct points
            setPoints(prevPoints => prevPoints.filter(point => point.pointId !== pointId));
            setFilteredPoints(prevFilteredPoints => prevFilteredPoints.filter(point => point.pointId !== pointId));
        } catch (error) {
            console.error(`Error when deleteing point ${pointId}: ${error.message}`);
            return error.message;
        }
    }

    //Creates comment and assigns it to point
    const createComment = async comment => {
        try {
            const retComment = await PointService.createComment(comment);
            // Adds comment to selected point and updates the points list
            const updatedComments = selectedPoint.comments.concat(retComment);
            setSelectedPoint({ ...selectedPoint, comments: updatedComments });
            setPoints(prevPoints =>
                prevPoints.map(p =>
                    p.pointId === comment.pointId
                        ? { ...p, comments: updatedComments }
                        : p
                )
            );

        } catch (error) {
            console.error(`Error creating comment: ${error.message}`);
            return error.message;
        }
    }

    const deleteComment = async comment => {
        try {
            await PointService.deleteComment(comment.pointId, comment.commentId);
            // Removes comment from selected point and updates the points list
            const updatedComments = selectedPoint.comments.filter(c => c.commentId !== comment.commentId);
            setSelectedPoint({ ...selectedPoint, comments: updatedComments });
            setPoints(prevPoints =>
                prevPoints.map(p =>
                    p.pointId === comment.pointId
                        ? { ...p, comments: updatedComments }
                        : p
                )
            );
        } catch (error) {
            console.error(`Error when deleteing comment ${comment.commentId} in ${comment.pointId}: ${error.message}`);
            return error.message;
        }
    }

    const deleteImage = async image => {
        if (window.confirm("Delete image? Image will be permanently deleted")) {
            try {
                await PointService.deleteImage(image.imageId);
                // Removes image from selected point and updates the points list
                const updatedImages = selectedPoint.images.filter(i => i.imageId !== image.imageId);
                const updatedPoint = { ...selectedPoint, images: updatedImages };
                setSelectedPoint(updatedPoint);
                setPoints(prevPoints =>
                    prevPoints.map(p =>
                        p.pointId === selectedPoint.pointId ? { ...p, images: updatedImages } : p
                    )
                );
            } catch (error) {
                console.error(`Error when deleteing image ${image.imageId} in ${image.pointId}: ${error.message}`);
                return error.message;
            }
        }
    }


    // The value object contains the state variables and functions that are available to components
    const value = {
        points,
        setPoints,
        filteredPoints,
        setFilteredPoints,
        selectedPoint,
        setSelectedPoint,
        latitude,
        setLatitude,
        longitude,
        setLongitude,
        fetchPoints,
        editPoint,
        deletePoint,
        createPoint,
        createComment,
        deleteComment,
        deleteImage,
        uploadedImages,
        setUploadedImages, 
    };

    // The provider wraps the child components that need access to the context
    // If testValue is provided, it is used instead of the actual value
    return (
        <PointContext.Provider value={testValue || value}>
            {children}
        </PointContext.Provider>
    );
};

// Custom hook that allows components to access the context
export const usePoint = () => useContext(PointContext);