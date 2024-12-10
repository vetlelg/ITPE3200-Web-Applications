import React, { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { useAccount } from '../../context/AccountContext';
import { usePoint } from '../../context/PointContext';
import { useUI } from '../../context/UIContext';
import { StarFill } from 'react-bootstrap-icons';

// Child component of CommentList

const CommentCreate = () => {
    const { accountId } = useAccount();
    const { selectedPoint, createComment } = usePoint();
    const { loggedIn } = useAccount();
    const { setShowLogin, setShowPoint } = useUI();
    const [Text, setText] = useState('');
    const [Rating, setRating] = useState(null);
    const [HoverRating, setHoverRating] = useState(null);

    // For the hover effect on the rating stars
    const handleHoverOn = (value) => {
        setHoverRating(value);
    }
    const handleHoverOff = () => {
        setHoverRating(null);
    }

    const submit = (event) => {
        //Prevents page from reseting when submitting form
        event.preventDefault();
        // Sets values of points if the user is logged in, if not, the user is redirected to login page
        if (loggedIn) {

            const comment = { commentId: null, text: Text, accountId: accountId, pointId: selectedPoint?.pointId, rating: parseInt(Rating) };
            
            //Sends comment to homepage
            createComment(comment);

            setText('');
            setRating(0);
            setHoverRating(null);
        }
        else {
            setShowPoint(false);
            setShowLogin(true);
        }
    };

    return (
        <div>
            <Form onSubmit={submit} >
                <Form.Group >
                    <Form.Label></Form.Label>
                    <Form.Control id='text' value={Text} onChange={(e) => setText(e.target.value)} required type="text" placeholder="Make a comment..." minLength="3" maxLength="100" title="Comment must be between 3 and 100 characters" />
                </Form.Group>
                <Form.Group className="mb-2">
                    <div className="d-flex align-items-center">
                        {[1, 2, 3, 4, 5].map((star) => (
                            <div key={star} className="position-relative mb-1" onMouseOver={() => handleHoverOn(star)} onMouseOut={handleHoverOff}>
                                <input
                                    type="radio"
                                    id={`star-${star}`}
                                    name="rating"
                                    value={star}
                                    onChange={() => setRating(star)}
                                    checked={Rating === star}
                                    required
                                    className="position-absolute opacity-0"
                                />
                                <label
                                    htmlFor={`star-${star}`}
                                    className={`me-2 ${star <= (HoverRating || Rating) ? 'text-warning' : 'text-muted'
                                        }`}
                                    style={{ fontSize: '1.5rem', cursor: 'pointer' }}
                                    
                                >
                                    <StarFill />
                                </label>
                            </div>
                        ))}
                    </div>
                </Form.Group>
                <Button className="btn btn-primary mb-2" type="submit">Create</Button>
            </Form>
        </div>
    )
}

export default CommentCreate;