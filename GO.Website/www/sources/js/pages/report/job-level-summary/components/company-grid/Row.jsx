import React from 'react';

import { renderNumber } from '../../common.funcs';

const Row = ({ data, jobType, totalJobType, showHideOptions, handleShowHideCells, onClick, isOther }) => {
    const renderCell = (value, jobLevel, country, gunit) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, country, gunit)}>{value}</div> : '';

    const isMultipleData = data.filter(obj => obj.name.slice(0, 2) === jobType).length > 1 || (isOther && data.length > 1);;

    const getRowSpanJobType = () => {
        let countColSpan = 0;
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) {
                countColSpan++;
            }
        });
        return isMultipleData ? countColSpan + 1 : countColSpan;
    };

    const getRowSpanJobTypeOther = () => data.length > 1 ? data.length + 1 : data.length;

    const renderTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if ((item.name.slice(0, 2) === jobType || isOther) && isMultipleData) {
                const cells = [];
                cells.push(<td key={`${item.name}-totalrow-name`} className="text-center col-job-level">Total</td>);
                let grandTotal = 0;
                item.cells.map(country => {
                    let totalCompany = 0;
                    if (country.name !== "Other") {
                        country.cells.map(gunit => {
                            cells.push(<td key={`${country.name}-${gunit.name}-${jobType}-total`} className="text-center col-total">{renderNumber(gunit.total)}</td>);
                            totalCompany += gunit.total;
                        });
                        country.cells.map(gunit => {
                            cells.push(<td key={`${country.name}-total-company-${jobType}-total`} className="text-center col-total">{renderNumber(totalCompany)}</td>);
                        });
                        grandTotal += totalCompany;
                    } else {
                        let totalUnitEmployee = 0;
                        country.cells.map(gunit => {
                            totalUnitEmployee += gunit.total;
                        });
                        cells.push(<td key={`${country.name}-${jobType}-total`} className="text-center col-total">{renderNumber(totalUnitEmployee)}</td>);
                        grandTotal += totalUnitEmployee;
                    }
                })
                cells.push(<td key={`grand-total-${jobType}`} className="text-center col-total">{renderNumber(grandTotal)}</td>);
                total.push(<tr key={`${item.name}-totalrow`}>{cells}</tr>);
            }
        });
        return total;
    }

    const renderbody = () => {
        const row = [];
        data.map(item => {
            if (item.name.slice(0, 2) === jobType || isOther) {
                const showHideOption = showHideOptions.find(x => x.key === jobType || (isOther && x.key === "Other"));
                const cells = [];
                if (row.length === 0) {
                    cells.push(<td key={`${item.name}-jobtype-row-name`} className={`${isMultipleData ? '' : 'text-center'} col-job-level bold`}
                        rowSpan={isOther ? getRowSpanJobTypeOther() : getRowSpanJobType()}>
                        {
                            isMultipleData &&
                            <span className={`fa ${showHideOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                onClick={() => handleShowHideCells(jobType)}/>
                        }
                        {isOther ? "Other" : jobType}
                    </td>);
                }

                if (showHideOption.show || !isMultipleData) {
                    cells.push(<td key={`${item.name}-jobtype-data`} className="text-center col-job-level">
                        {isMultipleData ? item.name : 'Total'}
                    </td>);
                    let grandTotal = 0;
                    item.cells.map(country => {
                        let totalCompany = 0;
                        if (country.name !== "Other") {
                            country.cells.map(unit => {
                                cells.push(<td key={`${item.name}-${country.name}-${unit.name}`} className={`${isMultipleData ? '' : 'col-total'} text-center`}>
                                    {renderCell(unit.total, item.name, country.name, unit.name)}
                                </td>);
                                totalCompany += unit.total;
                                cells.push(<td key={`${item.name}-${country.name}-${unit.name}-total-coutry`} className={`col-total text-center`}>{renderNumber(totalCompany)}</td>);
                            });
                            grandTotal += totalCompany;
                        } else {
                            let totalUnitEmployee = 0;
                            country.cells.map(unit => {
                                totalUnitEmployee += unit.total;
                            });
                            cells.push(<td key={`${country.name}-${jobType}-total`} className="text-center col-total">{renderNumber(totalUnitEmployee)}</td>);
                            grandTotal += totalUnitEmployee;
                        }
                    })
                    cells.push(<td key={`grand-total-${jobType}-${item.name}`} className="text-center col-total">{renderNumber(grandTotal)}</td>);
                }
                row.push(<tr key={`${item.name}-row-data`}>{cells}</tr>);
            }
        });
        return row;
    }

    return (
        <>
            {renderbody()}
            {renderTotal()}
        </>
    );
};

export default Row;