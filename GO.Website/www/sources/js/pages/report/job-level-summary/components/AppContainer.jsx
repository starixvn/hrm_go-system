import React, { Component } from 'react';
import { Button, Overlay, Popover, Form } from 'react-bootstrap';
import DataService from '@jsroot/common/dataService';
import UnitGrid from './unit-grid/index';
import CompanyGrid from './company-grid/index';
import CountryGrid from './country-grid/index';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';
import DatePicker from '@jsroot/components/date-picker/index';

export default class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeInfos: [],
            gridCountryData: null,
            gridUnitData: null,
            gridCompanyData: null,
            joinedDate: new Date(),
            jobLevelTypes: {
                Supervisor: [],
                Employee: [],
                Other: []
            },
            detail: {
                show: false,
                target: null,
                employeeInfos: []
            }
        };
    }

    async componentDidMount() {
        await this.getReportData();
    }

    getReportData = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('JobLevelSummaryReport/getData');
        const param = {
            JoinedDate: this.state.joinedDate
        }
        await DataService.postDataAsync(url, param).then(response => {
            this.setState({
                ...response.data
            });
        }).then(() => Site.unblockUI());
    };

    exportExcel = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('JobLevelSummaryReport/exportExcel');
        await DataService.getFileAsync(url).then(response => {
            const fileName = response.headers['content-disposition'].split('filename=')[1].split(';')[0];
            Site.downloadFile(response.data, fileName);
        }).then(() => Site.unblockUI());
    };

    handleShowDetail = (target, employeeInfos) => {
        this.setState({
            detail: {
                show: true,
                target,
                employeeInfos
            }
        });
    };

    handleHideDetail = () => {
        this.setState({
            detail: {
                show: false,
                target: null,
                employeeInfos: []
            }
        });
    };

    renderDetail = emps => {
        return (
            <table className="table table-employee-infos">
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
                                <td className="text-center">{index + 1}</td>
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

    render() {
        const { employeeInfos, gridCountryData, gridUnitData, gridCompanyData, jobLevelTypes, detail, joinedDate } = this.state;

        return (
            <div className="manpower-report">
                <Form className="form-filter">
                    <Form.Row bsPrefix="form-row">
                        <Form.Group className="filter-job-level" >
                            <DatePicker selected={joinedDate}
                                showMonthYearPicker
                                dateFormat="MMM yyyy"
                                onChange={value => this.setState({
                                    joinedDate: value
                                })}
                            />
                        </Form.Group>
                        <Form.Group className="form-group-width filter-job-level">
                            <Button variant="primary" size="sm" className="mr-3" onClick={this.getReportData}>
                                {LangHelper.getResource('Submit')}
                            </Button>
                            <Button variant="primary" size="sm" className="mr-3" onClick={this.exportExcel}>
                                {LangHelper.getResource('ExportExcel')}
                            </Button>
                        </Form.Group>
                    </Form.Row>
                </Form>

                {
                    gridUnitData &&
                    <UnitGrid
                        data={gridUnitData}
                        employeeInfos={employeeInfos}
                        detail={detail}
                        onShowDetail={this.handleShowDetail}
                        jobLevelTypes={jobLevelTypes}
                    />
                }
                {
                    gridCompanyData &&
                    <CompanyGrid
                        data={gridCompanyData}
                        employeeInfos={employeeInfos}
                        detail={detail}
                        onShowDetail={this.handleShowDetail}
                        jobLevelTypes={jobLevelTypes}
                    />
                }
                {
                    gridCountryData &&
                    <CountryGrid
                        data={gridCountryData}
                        employeeInfos={employeeInfos}
                        detail={detail}
                        onShowDetail={this.handleShowDetail}
                        jobLevelTypes={jobLevelTypes}
                    />
                }
                <Overlay
                    show={detail.show}
                    target={detail.target}
                    popperConfig={{ placement: 'auto' }}
                >
                    <Popover id="popover-employee-infos" className="popover-employee-infos">
                        <Popover.Title as="div"><i className="fa fa-times" onClick={this.handleHideDetail} /></Popover.Title>
                        <Popover.Content>
                            {this.renderDetail(detail.employeeInfos)}
                        </Popover.Content>
                    </Popover>
                </Overlay>
            </div>
        );
    }
}