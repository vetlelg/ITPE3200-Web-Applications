import Button from 'react-bootstrap/Button';
import Modal from "react-bootstrap/Modal";
import Form from 'react-bootstrap/Form';
import Image from 'react-bootstrap/Image';
import API_URL from '../../services/apiConfig';
import React, { useState } from 'react';
import { useUI } from '../../context/UIContext';
import { usePoint } from '../../context/PointContext';

const PointEdit = () => {
    const { showPointEdit, setShowPointEdit } = useUI();
    const { setShowPoint } = useUI();

    const { selectedPoint, editPoint, deleteImage, uploadedImages, setUploadedImages } = usePoint();

    const handleCloseClick = () => setShowPointEdit(false);

    const submit = async (event) => {
        //Prevents page from reseting when submitting form
        event.preventDefault();

        // Iterates though the list of image files and converts them into base64
        const imageList = [];
        const imageNameList = [];

        if (uploadedImages[0] != null) {
            for (let i = 0; i < uploadedImages[0].length; i++) {
                let res = await convertBase64(uploadedImages[0][i]);
                imageList[i] = res;
                imageNameList[i] = uploadedImages[0][i].name;
            }
        }

        // Sets values of points
        // Sets value to the new value if user has changed that feild, othervise use the original value
        const point =
        {
            pointId: selectedPoint?.pointId,
            comments: selectedPoint.comments,
            name: Name || selectedPoint?.name,
            latitude: selectedPoint?.latitude,
            longitude: selectedPoint?.longitude,
            description: Description || selectedPoint?.description,
            accountId: selectedPoint?.accountId,
            account: selectedPoint?.account,
            images: selectedPoint?.images,
            uploadedImages: [...imageList],
            uploadedImagesNames: [...imageNameList]
        }
        // Sends point to home page
        await editPoint(point);
        setShowPointEdit(false);
        setShowPoint(false)
        setUploadedImages([]);
    };

    // Converts an image file to base64 string
    async function convertBase64(image) {
        return new Promise((succeed) => {
            const reader = new FileReader();
            reader.readAsDataURL(image);
            reader.onload = () => succeed(reader.result);
        });
    }
    

    const [Name, setName] = useState(selectedPoint?.title);
    const [Description, setDescription] = useState(selectedPoint?.description);

    return (
        <Modal show={showPointEdit} onHide={handleCloseClick}>
            <Modal.Header closeButton>
                <Modal.Title>Edit marker</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={submit}>
                    <Form.Group className="mb-3">
                        <Form.Label>Title</Form.Label>
                        <Form.Control onChange={(e) => setName(e.target.value)} required defaultValue={selectedPoint?.name} type="text" pattern="[a-zA-Z0-9øæåØÆÅ ]*" minLength="3" maxLength="40" title="Point title must be between 3 and 20 characters. No special characters" />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Description</Form.Label>
                        <Form.Control onChange={(e) => setDescription(e.target.value)} required defaultValue={selectedPoint?.description} as="textarea" rows={3} placeholder="Enter Title" maxLength="150" title="Description must be less then 150 characters." />
                    </Form.Group>
                    <Form.Group className="mb-3">
                        <Form.Label>Images</Form.Label>
                        <Form.Control onChange={(e) => setUploadedImages([e.target.files])} type="file" multiple accept=".jpg, .jpeg, .png" />
                    </Form.Group>
                    {selectedPoint?.images.map(image =>
                        <div key={image.imageId}>
                            <Image className='mb-2 me-2 mt-2' src={`${API_URL}/${image.filePath}`} alt="Point" style={{ width: '100px', height: '100px' }} />
                            <Button className='btn btn-danger' onClick={() => deleteImage(image)}>Delete</Button>
                        </div>
                    )}
                    <Button className="btn btn-primary" type="submit">Edit</Button>
                </Form>
            </Modal.Body>
        </Modal>
    );
};

export default PointEdit;