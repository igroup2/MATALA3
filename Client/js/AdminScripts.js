$(document).ready(function () {
  let currentTable = "users"; // Track which table is currently displayed
  let dataTable; // Single DataTable instance

  const userColumns = [
    { data: "id", title: "ID" },
    { data: "name", title: "User Name" },
    { data: "numOfGamesBought", title: "Number of Games Purchased" },
    { data: "totalSpent", title: "Total Money Spent ($)" },
    {
      data: "isActive",
      title: "Active Status",
      render: function (data, type, row) {
        return `<input type="checkbox" class="active-checkbox" data-id="${
          row.id
        }" 
        ${data ? "checked" : ""} />`;
      },
    },
  ];

  $("#dataTable").on("change", ".active-checkbox", function () {
    const userId = $(this).data("id"); // Get the user ID
    const isActive = $(this).is(":checked"); // true if checked, false if unchecked
    console.log(userId, isActive);
    let api = `https://proj.ruppin.ac.il/igroup2/test2/tar1/api/Users/UpdateUserStatus?isActive=${isActive}`;
    // https://localhost:7287/api/Users/UpdtaeUserStatus?isActive=false
    ajaxCall(
      "PUT",
      api,
      JSON.stringify(userId),
      function (response) {
        console.log(response);
      },
      function (error) {
        console.error(error);
      }
    );
  });

  const gameColumns = [
    { data: "appID", title: "Game ID" },
    { data: "name", title: "Game" },
    { data: "numberOfPurchases", title: "Number of Downloads" },
    { data: "totalSpent", title: "Total Revenue ($)" },
  ];

  // Initialize DataTable with empty data and columns
  dataTable = $("#dataTable").DataTable({
    data: [],
    columns: userColumns, // Default to user columns
    pageLength: 5,
  });

  // Fetch and load initial data for Users
  fetchAndLoadTable("users");

  // Function to fetch and load data for the table
  function fetchAndLoadTable(tableType) {
    const apiUrl =
      tableType === "users"
        ? "https://proj.ruppin.ac.il/igroup2/test2/tar1/api/Users/SpecificUserInfo"
        : "https://proj.ruppin.ac.il/igroup2/test2/tar1/api/Games/SpecificGameInfo";

    const columns = tableType === "users" ? userColumns : gameColumns;

    ajaxCall(
      "GET",
      apiUrl,
      "",
      (data) => {
        if (data && Array.isArray(data)) {
          updateTable(data, columns);
        } else {
          console.error(`Invalid data received for ${tableType}`);
        }
      },
      (error) => console.error(`Error fetching ${tableType} data:`, error)
    );
  }

  // Function to update the table with new data and columns
  function updateTable(data, columns) {
    if ($.fn.DataTable.isDataTable("#dataTable")) {
      dataTable.clear(); // Clear existing data
      dataTable.destroy(); // Destroy the current DataTable instance
      $("#dataTable thead").empty(); // Clear existing headers
    }

    // Set new headers dynamically
    const headerHtml = columns.map((col) => `<th>${col.title}</th>`).join("");
    $("#dataTable thead").html(`<tr>${headerHtml}</tr>`);

    // Reinitialize DataTable with new data and columns
    dataTable = $("#dataTable").DataTable({
      data: data,
      columns: columns,
      pageLength: 5,
    });
  }

  // Handle table switching
  $("#tableSwitcher").on("click", function () {
    if (currentTable === "users") {
      currentTable = "games";
      fetchAndLoadTable("games");
      $(this).text("Switch to Users Table");
    } else {
      currentTable = "users";
      fetchAndLoadTable("users");
      $(this).text("Switch to Games Table");
    }
  });

  // Handle search functionality
  $("#searchInput").on("keyup", function () {
    dataTable.search(this.value).draw();
  });

  // Handle row display length change
  $("#rowCount").on("change", function () {
    dataTable.page.len(this.value).draw();
  });

  // Handle pagination
  $("#prevPage").on("click", function () {
    dataTable.page("previous").draw("page");
  });

  $("#nextPage").on("click", function () {
    dataTable.page("next").draw("page");
  });
});
