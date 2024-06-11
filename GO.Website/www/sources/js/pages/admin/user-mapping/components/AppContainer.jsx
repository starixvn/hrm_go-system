import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { Button } from 'react-bootstrap';

import Filter from './Filter';
import DataGrid from './grid/DataGrid';
import AddSubUserModalDialog from './AddSubUserModalDialog';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = () => {
    const [filterParams, setFilterParams] = useState({ keyword: '' });
    const [users, setUsers] = useState([]);
    const [addSubUserModalDialog, setAddSubUserModalDialog] = useState({ show: false, selectedUser: null });
    const [deleteSubUserModalDialog, setDeleteSubUserModalDialog] = useState({ show: false, mainUser: null, subUser: null });

    const getUserMapping = params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Admin/Usermapping/GetUserMapping');
        DataService.getDataAsync(url, params).then(response => {
            setUsers(response.data);
        }).then(() => Site.unblockUI());
    };

    useEffect(() => {
        getUserMapping(filterParams);
    }, []);

    const handleGetUserMapping = params => {
        setFilterParams(params);
        getUserMapping(params);
    };

    const handleAddUserMapping = formData => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Admin/UserMapping/Create');
        DataService.postDataAsync(url, formData).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Add user mapping successfully!');

                setAddSubUserModalDialog({ show: false, selectedUser: null });
                getUserMapping(filterParams);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    const handleOpenDeleteSubUserModalDialog = (mainUser, subUser) => setDeleteSubUserModalDialog({ show: true, mainUser, subUser });

    const handleCloseDeleteSubUserModalDialog = () => setDeleteSubUserModalDialog({ show: false });

    const deleteSubUser = () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Admin/UserMapping/Delete');
        const formData = {
            userId: deleteSubUserModalDialog.subUser.userId,
            linkToUserId: deleteSubUserModalDialog.mainUser.userId
         };
        DataService.postDataAsync(url, formData).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete user mapping successfully!');
                getUserMapping(filterParams);
                handleCloseDeleteSubUserModalDialog();
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    return (
        <>
            <Filter onSubmit={handleGetUserMapping}
                onAddSubUser={setAddSubUserModalDialog}
            />
            <DataGrid users={users}
                onAddSubUser={setAddSubUserModalDialog}
                onDeleteSubUser={handleOpenDeleteSubUserModalDialog}
            />
            {
                addSubUserModalDialog.show &&
                <AddSubUserModalDialog selectedUser={addSubUserModalDialog.selectedUser}
                    onSubmit={handleAddUserMapping}
                    onClose={setAddSubUserModalDialog}
                />
            }
            {
                deleteSubUserModalDialog.show &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to delete sub-user <strong>{deleteSubUserModalDialog.subUser.fullName} ({deleteSubUserModalDialog.subUser.nickname})</strong>?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={deleteSubUser}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={handleCloseDeleteSubUserModalDialog}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={handleCloseDeleteSubUserModalDialog}
                />
            }
        </>
    );
};

export default AppContainer;