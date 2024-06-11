import React, { useState } from 'react';
import { Alert, Button, Col, Form, Modal, Row } from 'react-bootstrap';
import Select from 'react-select';
import { isEmpty } from 'lodash';

import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';
import DataService from '@jsroot/common/dataService';

const AddSubUserModalDialog = ({ selectedUser, onSubmit, onClose }) => {
    const [mainUser, setMainUser] = useState(selectedUser);
    const [subUser, setSubUser] = useState(null);
    const [mainUserOptions, setMainUserOptions] = useState([]);
    const [subUserOptions, setSubUserOptions] = useState([]);
    const [errors, setErrors] = useState({});

    const handleClose = () => {
        onClose({ show: false, selectedUser: null })
    };

    const handleSearchUser = Site.debounce((keyword, isMain) => {
        if (keyword.length >= 2) {
            const url = Site.resolveClientUrl('Admin/UserMapping/SearchUser');
            DataService.getDataAsync(url, { keyword, type: isMain ? 1 : 2 }).then(response => {
                const users = response.data.map(item => {
                    return {
                        ...item,
                        value: `${item.companyId}-${item.empId}`,
                        label: `${item.fullName} (${item.nickname})`
                    };
                });

                if (isMain) {
                    setMainUserOptions(users);
                } else {
                    setSubUserOptions(users);
                }
            });
        } else {
            if (isMain && mainUserOptions.length > 0) {
                setMainUserOptions([]);
            }

            if (!isMain && subUserOptions.length > 0) {
                setSubUserOptions([]);
            }
        }
    });

    const validate = () => {
        let isValid = true;
        let errs = {};
        if (mainUser == null) {
            isValid = false;
            errs['mainUser'] = 'Main user is required.';
        }

        if (subUser == null) {
            isValid = false;
            errs['subUser'] = 'Sub-user is required.';
        }

        setErrors(errs);

        return isValid;
    };

    const handleSubmit = () => {
        if (validate()) {
            const formData = {
                userId: subUser.userId,
                linkToUserId: mainUser.userId
            };
            onSubmit(formData);
        }
    };

    const formatOptionLabel = option => (
        <>
            <div>
                <span className="font-weight-bold">{LangHelper.getResource('Company')}</span>
                : <span className="font-italic">{option.companyName}</span>
            </div>
            <div>
                <span className="font-weight-bold">{LangHelper.getResource('Code')}</span>
                : <span className="font-italic">{option.code}</span>
            </div>
            <div>
                <span className="font-weight-bold">{LangHelper.getResource('Nickname')}</span>
                : <span className="font-italic">{option.nickname}</span>
            </div>
            <div>
                <span className="font-weight-bold">{LangHelper.getResource('FullName')}</span>
                : <span className="font-italic">{option.fullName}</span>
            </div>
        </>
    );

    const handleAddMainUser = item => setMainUser(item);

    const handleAddSubUser = item => setSubUser(item);

    const renderUserInfo = user => {
        return (
            <div className="user-info">
                <div><span className="font-weight-bold">Company</span>: <span className="font-italic">{user ? user.companyName : ''}</span></div>
                <div><span className="font-weight-bold">Code</span>: <span className="font-italic">{user ? user.code : ''}</span></div>
                <div><span className="font-weight-bold">Nickname</span>: <span className="font-italic">{user ? user.nickname : ''}</span></div>
                <div><span className="font-weight-bold">Full Name</span>: <span className="font-italic">{user ? user.fullName : ''}</span></div>
            </div>
        );
    };

    return (
        <Modal show backdrop="static" keyboard={false} size="sm" centered onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title as="div">Add user mapping</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form id="user-mapping-form" className="user-mapping-form" noValidate>
                    {
                        !isEmpty(errors) &&
                        <Alert variant='danger'>
                            <ul className='list-message-error'>
                                {
                                    Object.keys(errors).map(field => <li key={field}>{errors[field]}</li>)
                                }
                            </ul>
                        </Alert>
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className='font-weight-bold'>Main user</Form.Label>
                        <Col sm="8" md="9">
                            {
                                selectedUser == null &&
                                <Select className='react-select-container'
                                    classNamePrefix="react-select"
                                    value={null} options={mainUserOptions}
                                    formatOptionLabel={formatOptionLabel}
                                    isClearable={false} placeholder="nickname or full name"
                                    backspaceRemovesValue={false}
                                    components={{ NoOptionsMessage: () => null, IndicatorSeparator: () => null, DropdownIndicator: () => null }}
                                    onInputChange={keyword => handleSearchUser(keyword, true)}
                                    onChange={handleAddMainUser}
                                />
                            }
                            {renderUserInfo(mainUser)}
                        </Col>
                    </Form.Group>
                    <hr />
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className='font-weight-bold'>Sub-user</Form.Label>
                        <Col sm="8" md="9">
                            <Select className='react-select-container'
                                classNamePrefix="react-select"
                                value={null} options={subUserOptions}
                                formatOptionLabel={formatOptionLabel}
                                isClearable={false} placeholder="nickname or full name"
                                backspaceRemovesValue={false}
                                components={{ NoOptionsMessage: () => null, IndicatorSeparator: () => null, DropdownIndicator: () => null }}
                                onInputChange={keyword => handleSearchUser(keyword, false)}
                                onChange={handleAddSubUser}
                            />
                            {renderUserInfo(subUser)}
                        </Col>
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" size="sm" onClick={handleSubmit}>{LangHelper.getResource('Submit')}</Button>
                <Button variant="secondary" size="sm" onClick={handleClose}>{LangHelper.getResource('Cancel')}</Button>
            </Modal.Footer>
        </Modal>
    );
};

export default AddSubUserModalDialog;