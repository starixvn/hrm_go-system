import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ onAddSubUser, onSubmit }) => {
    const [keyword, setKeyword] = useState('');

    const handleSubmit = event => {
        event.preventDefault();

        onSubmit({keyword});
    };

    const handleChange = event => setKeyword(event.target.value);

    const handleAddSubUser = () => onAddSubUser({ show: true, selectedUser: null });

    return (
        <Form className="form-filter" onSubmit={handleSubmit}>
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Keyword')}</Form.Label>
                    <Form.Control type="text" size="sm" value={keyword}
                        name="keyword" id="keyword" placeholder='nickname or full name'
                        className="shadow-none" autoComplete="off"
                        onChange={handleChange}
                    />
                </Form.Group>
                <Form.Group className='form-btn-group'>
                    <Button variant="primary" size="sm" className="mr-3" type="button" onClick={handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                    {
                        window.hasHostPermission &&
                        <Button variant="primary" size="sm" className="mr-3" type="button" onClick={handleAddSubUser}>
                            Add user mapping
                        </Button>
                    }
                </Form.Group>
            </Form.Row>
        </Form>
    );
}

export default Filter;