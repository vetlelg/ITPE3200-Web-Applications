import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import PointShow from '../components/Point/PointShow';
import { UIProvider } from '../context/UIContext';
import { PointProvider } from '../context/PointContext';
import { AccountProvider } from '../context/AccountContext';

// Mock functions and real values for testing PointShow component
const mockUIContextValue = {
    showPoint: true,
    setShowPoint: jest.fn(),
    setShowImageCarousel: jest.fn(),
    setShowPointEdit: jest.fn(),
};

const mockPointContextValue = {
    selectedPoint: {
        pointId: 4,
        name: 'Test Name',
        description: 'Test Description',
        latitude: 10,
        longitude: 20,
        account: { email: 'admin@admin.com' },
        accountId: 1,
        images: [{ imageId: 1, filePath: "/test/test.jpg"}],
        comments: [{ text: "Test comment", rating: 3, account: { email: 'admin@admin.com' } }, { text: "Test comment2", rating: 4, account: { email: 'user@user.com' } }],
    },
    deletePoint: jest.fn(),
};

const mockAccountContextValue = {
    accountId: 1,
};

// Complete tests for the PointShow component
describe('Tests PointShow Component', () => {
    beforeEach(() => {
        render(
            // This is neccesery to give the PointShow component the correct context values.
            // Sets up PointShow component for all the unit tests.
            // The UIProvidor, AccountProvider and PointProvider are contexts and they give the Pontshow component
            // test values and mocked components 
            <UIProvider testValue={mockUIContextValue}>
                <AccountProvider testValue={mockAccountContextValue}>
                    <PointProvider testValue={mockPointContextValue}>
                        <PointShow />
                    </PointProvider>
                </AccountProvider>
            </UIProvider>
        );
    });

// These tests test that the Point component and functoanlity works correctly
    // Ensures that the Point name is displayed correctly
    test('Test  Point name value', () => {
        const name = screen.getByText('Test Name')
        expect(name).toBeInTheDocument();
    });

    //Ensures that the combined rating of comment 1 and comment 2 is 3.50 as expected
    test('Test average rating value', () => {
        const rating = screen.getByText('Rating: 3.50')
        expect(rating).toBeInTheDocument();
    })

    // Ensures that the correct description value is displayed
    test('Test description value', () => {
        const description = screen.getByText('Description: Test Description')
        expect(description).toBeInTheDocument();
    })

    // Ensures that the feild coorectly combines and displayes the latitude and longitude information
    test('Test description value', () => {
        const position = screen.getByText('Position: (10, 20)')
        expect(position).toBeInTheDocument();
    })

    // Ensures that the correct Account value is displayed
    test('Test description value', () => {
        const account = screen.getByText('Account: admin@admin.com')
        expect(account).toBeInTheDocument();
    })

    
    // Ensures that when modal closebutton is clicked it sets setShowPoint to false
    test('modal closebutton sets showPoint to false', () => {
        fireEvent.click(screen.getByRole('button', { name: /close/i }));
        expect(mockUIContextValue.setShowPoint).toHaveBeenCalledWith(false);
    });

    // Ensures that when an image is cliked the ImageCarousel is shown
    test('Image click shows ImageCarusel', () => {
        // Clickes on the image
        fireEvent.click(screen.getByRole('img'));
        expect(mockUIContextValue.setShowImageCarousel).toHaveBeenCalledWith(true);
    });

    // When delete button is clicked and user clicks confirm in the confirm window, the deletePoint function is called
    test('Delete point click and confirm calls delete point method in context', () => {
        // Mocks window.confirm to return true because of the pop up confirm window which occures on delete
        window.confirm = jest.fn().mockImplementation(() => true);
        fireEvent.click(screen.getByRole('button', { name: /delete/i }));

        // Checks that the pointid value is 4
        expect(mockPointContextValue.deletePoint).toHaveBeenCalledWith(4);
    });

    // When delete button is clicked and user clicks cancel in the confirm window, the deletePoint function is called
    test('Delete point click and cancel does not call delete point method in context', () => {
        // Mocks window.confirm to return false because of the pop up confirm window which occures on delete
        window.confirm = jest.fn().mockImplementation(() => false);
        fireEvent.click(screen.getByRole('button', { name: /delete/i }));

        // Checks that deletePoint has not been called
        expect(mockPointContextValue.deletePoint).not.toHaveBeenCalled();
    });

    // When edit button is clicked, the edit point page is shown
    test('handleEditClick sets showPointEdit to true', () => {
        // Clickes on the edit button
        fireEvent.click(screen.getByRole('button', { name: /edit/i }));
        expect(mockUIContextValue.setShowPointEdit).toHaveBeenCalledWith(true);
    });
});