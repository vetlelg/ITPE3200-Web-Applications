import Button from 'react-bootstrap/Button';
import Modal from "react-bootstrap/Modal";
import Form from 'react-bootstrap/Form';
import React, { useState } from 'react';
import { useUI } from '../../context/UIContext';
import { useAccount } from '../../context/AccountContext';
import { usePoint } from '../../context/PointContext';

const PointCreate = () => {
    const { showPointCreate, setShowPointCreate } = useUI();
    const { accountId } = useAccount();
    const { latitude, longitude, createPoint } = usePoint();
    
    const handleCloseClick = () => setShowPointCreate(false);

    const submit = async (event) => {
        //Prevents page from reseting when submitting form
        event.preventDefault();

        // Iterates though the list of image files and converts them into base64
        const imageList = [];
        const imageNameList = [];
        if (UploadedImages[0] != null) {
            for (let i = 0; i < UploadedImages[0].length; i++) {
                let res = await convertBase64(UploadedImages[0][i]);
                imageList[i] = res;
                imageNameList[i] = UploadedImages[0][i].name;
            }
        }
        

        // Sets values of points
        const point =
        {
            pointId: null,
            comments: [],
            name: Name,
            latitude: latitude,
            longitude: longitude,
            description: Description,
            accountId: accountId,
            uploadedImages: [...imageList],
            uploadedImagesNames: [...imageNameList]
        }
        // Sends point to home page
        createPoint(point);
        setShowPointCreate(false);
    };

    // Converts an image file to base64 string
    async function convertBase64(image) {
        return new Promise((succeed) => {
            const reader = new FileReader();
            reader.readAsDataURL(image);
            reader.onload = () => succeed(reader.result);
        });
    }

    const [Name, setName] = useState('');
    const [Description, setDescription] = useState(0);
    const [UploadedImages, setUploadedImages] = useState([]);

    return (
        <Modal show={showPointCreate} onHide={handleCloseClick}>
            <Modal.Header closeButton>
                <Modal.Title>Create a new marker</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={submit}>
                    <Form.Group className="mb-3">
                        <Form.Label>Title</Form.Label>
                        <Form.Control onChange={(e) => setName(e.target.value)} required type="text" placeholder="Enter Title" pattern="[a-zA-Z0-9øæåØÆÅ ]*" minLength="3" maxLength="40" title="Point title must be between 3 and 40 characters. No special characters" />

                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Description</Form.Label>
                        <Form.Control onChange={(e) => setDescription(e.target.value)} required as="textarea" rows={3} placeholder="Enter Title" maxLength="150" title="Description must be less then 150 characters. No special characters" />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Images</Form.Label>
                        <Form.Control onChange={(e) => setUploadedImages([e.target.files])} type="file" multiple accept=".jpg, .jpeg, .png" />
                    </Form.Group>

                    <Button className="btn btn-primary" type="submit">Create</Button>
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default PointCreate;