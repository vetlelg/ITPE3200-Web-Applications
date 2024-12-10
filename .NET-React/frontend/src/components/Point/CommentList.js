import Table from 'react-bootstrap/Table';
import { StarFill, Star } from 'react-bootstrap-icons';
import Button from 'react-bootstrap/Button';
import CommentCreate from './CommentCreate'
import { useAccount } from '../../context/AccountContext';
import { useUI } from '../../context/UIContext';
import { usePoint } from '../../context/PointContext';

// Child component of PointShow

const CommentList = () => {
    const { setShowPoint, setShowLogin } = useUI();
    const { selectedPoint, createComment, deleteComment } = usePoint();
    const { accountId, loggedIn } = useAccount();

    const handleDeleteClick = async comment => {
        if (window.confirm("Delete comment? Comment will be permanently deleted"))
            deleteComment(comment);
    }

    const handleCreateClick = async comment => {
        if (loggedIn)
            createComment(comment);
        else {
            setShowPoint(false);
            setShowLogin(true);
        }
    }

    let PrintRating = (rating) =>
    {
        let elements = [];
        for (var i = 1; i <= 5; i++) {
            if (rating >= i) {

                elements.push(<StarFill key={i}  className='text-warning'/>);
                
            }
            else {
                elements.push(<Star key={i} />);
            }
        }
        return elements;
    };
    const showButton = (comment) => {
        let elements = [];

        // If the correct user is logged in, then edit and delete buttons are shown.
        if (comment?.accountId === accountId && accountId != null)
        {
            elements.push(<td key="1"><Button onClick={() => handleDeleteClick(comment)} className="btn btn-danger mt-1">Delete</Button></td>)
        }
        return elements;
    }

    return (
        <div className="col-12 col-md-12 col-lg-4" style={{ width: "100%" }}>
            <h3>Comments</h3>
            <Table striped responsive>
                <thead>
                </thead>
                <tbody>
                    {
                        selectedPoint?.comments?.map(comment =>
                        (
                            <tr key={comment.commentId}>
                                <td><span className="fw-bold">{comment.account.email}</span>: {comment.text}
                                <br />
                                    Rating: {PrintRating(comment.rating)}</td>
                                {showButton(comment)}
                            </tr>
                        ))
                    }  
                </tbody>
            </Table>
            <div className='sticky-bottom px-2 bg-body text-body'>
            <CommentCreate accountId={accountId} selectedPoint={selectedPoint} onCommentCreate={handleCreateClick} />
            </div>
        </div>

    )
}

export default CommentList;
