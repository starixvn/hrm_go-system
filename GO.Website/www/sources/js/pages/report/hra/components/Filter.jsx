import React, { useState } from "react";
import { Button, Form } from "react-bootstrap";
import Select from "react-select";

import LangHelper from "@jsroot/common/langHelper";
import PropTypes from "prop-types";

const Filter = ({ exportExcel }) => {
  const currentYear = new Date().getFullYear();
  const years = Array.from(
    { length: currentYear - 2011 + 1 },
    (_, index) => currentYear - index
  );

  const [selectedYear, setSelectedYear] = useState({
    value: currentYear,
    label: currentYear,
  });

  const handleYearChange = (event) => {
    setSelectedYear({
      value: event.value,
      label: event.value,
    });
  };

  const handleExportExcel = (event) => {
    event.preventDefault();

    exportExcel(selectedYear.value);
  };

  const options = years.map((item) => {
    return {
      value: item,
      label: item,
    };
  });

  return (
    <Form className="form-filter">
      <Form.Row bsPrefix="form-row">
        <Form.Group>
          <Form.Label>Year</Form.Label>
          <Select
            className={`react-select-container`}
            classNamePrefix="react-select"
            options={options}
            value={selectedYear}
            isSearchable={false}
            onChange={handleYearChange}
          />
        </Form.Group>
        <Form.Group className="form-group-width">
          <Button
            variant="primary"
            size="sm"
            className="mr-3"
            type="submit"
            onClick={handleExportExcel}>
            {LangHelper.getResource("ExportExcel")}
          </Button>
        </Form.Group>
      </Form.Row>
    </Form>
  );
};

Filter.propTypes = {
  exportExcel: PropTypes.func.isRequired,
};

export default Filter;
