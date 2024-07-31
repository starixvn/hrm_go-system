import React from 'react';

import { renderNumber } from '../../common.funcs';

const TotalRow = ({ data }) => {
    const renderTotalCompany = () => {
        let cells = [];
        let grandTotal = 0;
        data.cells.map(country => {
            let totalSub = 0;
            if (country.name !== "Other") {
                country.cells.map(unit => {
                    cells.push(<td key={`total-super-${country.name}-${unit.name}`} className="text-center">{renderNumber(unit.total)}</td>);
                    totalSub += unit.total;
                });
                cells.push(<td key={`total-super-company-${country.name}`} className="text-center">{renderNumber(totalSub)}</td>);
            } else {
                let totalEmployee = 0;
                country.cells.map(unit => {
                    totalEmployee += unit.total;
                });
                cells.push(<td key={`total-super-${country.name}`} className="text-center">{renderNumber(totalEmployee)}</td>);
                grandTotal += totalEmployee;
            }
            grandTotal += totalSub;
        })
        cells.push(<td key={`grand-total-row-company-${data.name}`} className="text-center">{renderNumber(grandTotal)}</td>);

        return cells;
    };

    return (
        <>
            <tr className="row-total">
                <td className="text-center" colSpan={2} >{data.name}</td>
                {
                    renderTotalCompany()
                }
            </tr>
        </>
    );
};

export default TotalRow;