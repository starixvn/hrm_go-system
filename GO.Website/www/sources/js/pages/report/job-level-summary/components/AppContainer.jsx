import React, { useEffect, useState } from 'react';
import { Button, Overlay, Popover, Form } from 'react-bootstrap';

import UnitGrid from './unit-grid/index';
import CompanyGrid from './company-grid/index';
import CountryGrid from './country-grid/index';
import DatePicker from '@jsroot/components/date-picker/index';

import DateTimeService from '@jsroot/common/DateTimeService';
import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = () => {
    const [filterParam, setFilterParam] = useState({ joinedDate: new Date() });
    const [reportData, setReportData] = useState({
        employeeInfos: [],
        gridCountryData: null,
        gridUnitData: null,
        gridCompanyData: null,
        jobLevelTypes: {
            Supervisor: [],
            Employee: [],
            Other: []
        }
    });
    const [detailPopover, setDetailPopover] = useState({
        show: false,
        target: null,
        employeeInfos: []
    });

    useEffect(() => {
        getReportData();
    }, []);

    const getReportData = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('JobLevelSummaryReport/getData');
        const param = {
            joinedDate: DateTimeService.toShortDateFormat(DateTimeService.firstDayOfMonth(filterParam.joinedDate))
        }
        await DataService.getDataAsync(url, param).then(response => {
            setReportData(response.data);
        }).then(() => Site.unblockUI());
    };

    /*const exportExcel = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('JobLevelSummaryReport/exportExcel');
        const param = {
            joinedDate: DateTimeService.toShortDateFormat(DateTimeService.firstDayOfMonth(filterParam.joinedDate))
        };
        await DataService.getFileAsync(url, param).then(response => {
            const fileName = response.headers['content-disposition'].split('filename=')[1].split(';')[0];
            Site.downloadFile(response.data, fileName);
        }).then(() => Site.unblockUI());
    };*/

    const handleShowDetail = (target, employeeInfos) => {
        setDetailPopover({
            show: true,
            target,
            employeeInfos
        });
    };

    const handleHideDetail = () => {
        setDetailPopover({
            show: false,
            target: null,
            employeeInfos: []
        });
    };

    const renderDetail = emps => {
        return (
            <table className='table table-employee-infos'>
                <thead>
                    <tr>
                        <td>#</td>
                        <td>{LangHelper.getResource('Company')}</td>
                        <td>{LangHelper.getResource('Code')}</td>
                        <td>{LangHelper.getResource('Nickname')}</td>
                        <td>{LangHelper.getResource('FullName')}</td>
                        <td>{LangHelper.getResource('JobLevel')}</td>
                        <td>{LangHelper.getResource('Category')}</td>
                        <td>{LangHelper.getResource('Unit')}</td>
                        <td>{LangHelper.getResource('Department')}</td>
                        <td>{LangHelper.getResource('Section')}</td>
                        <td>{LangHelper.getResource('Team')}</td>
                        <td>{LangHelper.getResource('Country')}</td>
                    </tr>
                </thead>
                <tbody>
                    {
                        emps.map((emp, index) =>
                            <tr key={`${emp.siteName}-${emp.nickname}`}>
                                <td className='text-center'>{index + 1}</td>
                                <td>{emp.companyName}</td>
                                <td>{emp.code}</td>
                                <td>{emp.nickname}</td>
                                <td>{emp.fullName}</td>
                                <td>{emp.jobLevel}</td>
                                <td>{emp.isLocal ? 'Local' : 'Expat'}</td>
                                <td>{emp.unitName}</td>
                                <td>{emp.departmentName}</td>
                                <td>{emp.sectionName}</td>
                                <td>{emp.teamName}</td>
                                <td>{emp.country}</td>
                            </tr>
                        )
                    }
                </tbody>
            </table>
        );
    };

    const { employeeInfos, gridCountryData, gridUnitData, gridCompanyData, jobLevelTypes } = reportData;

    return (
        <div className='manpower-report'>
            <Form className='form-filter'>
                <Form.Row bsPrefix='form-row'>
                    <Form.Group className='filter-job-level' >
                        <DatePicker selected={filterParam.joinedDate}
                            showMonthYearPicker
                            dateFormat='MMM yyyy'
                            onChange={value => setFilterParam({
                                ...filterParam,
                                joinedDate: value
                            })}
                        />
                    </Form.Group>
                    <Form.Group className='form-group-width filter-job-level'>
                        <Button variant='primary' size='sm' className='mr-3' onClick={getReportData}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>

            {
                gridUnitData &&
                <UnitGrid
                    data={gridUnitData}
                    employeeInfos={employeeInfos}
                    detail={detailPopover}
                    onShowDetail={handleShowDetail}
                    jobLevelTypes={jobLevelTypes}
                />
            }
            {
                gridCompanyData &&
                <CompanyGrid
                    data={gridCompanyData}
                    employeeInfos={employeeInfos}
                    detail={detailPopover}
                    onShowDetail={handleShowDetail}
                    jobLevelTypes={jobLevelTypes}
                />
            }
            {
                gridCountryData &&
                <CountryGrid
                    data={gridCountryData}
                    employeeInfos={employeeInfos}
                    detail={detailPopover}
                    onShowDetail={handleShowDetail}
                    jobLevelTypes={jobLevelTypes}
                />
            }

            <Overlay
                show={detailPopover.show}
                target={detailPopover.target}
                popperConfig={{ placement: 'auto' }}
            >
                <Popover id='popover-employee-infos' className='popover-employee-infos'>
                    <Popover.Title as='div'><i className='fa fa-times' onClick={handleHideDetail}/></Popover.Title>
                    <Popover.Content>
                        {renderDetail(detailPopover.employeeInfos)}
                    </Popover.Content>
                </Popover>
            </Overlay>
        </div>
    );
};

export default AppContainer;