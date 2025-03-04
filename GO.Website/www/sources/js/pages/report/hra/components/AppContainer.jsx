import React from "react";
import Filter from "./Filter";

const AppContainer = () => {
  const exportExcel = async (year) => {
    window.location.href = `exportExcel?year=${year}`;
  };

  return (
    <div className="hra-report">
      <Filter exportExcel={exportExcel} />
    </div>
  );
};

export default AppContainer;
