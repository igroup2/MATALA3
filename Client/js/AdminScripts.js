// This file contains the JavaScript code for initializing DataTables, handling AJAX calls, and managing UI interactions.
$(document).ready(function () {
  let currentTable = "users"; // Track which table is currently displayed
  let usersTable, gamesTable;
  let api = "https://localhost:7287/api/Users/SpecificUserInfo";

  // Fetch data using your ajaxcall
  ajaxCall(
    "GET",
    api,
    "",
    (data) => initializeUsersTable(data), // Success callback
    (error) => console.log("Error fetching users data:", error) // Error callback
  );

  // Initialize DataTables
  function initializeUsersTable(data) {
    usersTable = $("#usersTable").DataTable({
      data: data,
      columns: [
        { data: "id", title: "ID" },
        { data: "name", title: "User Name" },
        { data: "numOfGamesBought", title: "Number Of Games purchesed" },
        { data: "totalSpent", title: "Total Money Spent ($)" },
        {
          data: "isActive",
          title: "Active Status",

          render: function (data, type, row, meta) {
            return data
              ? '<input type="checkbox" checked disabled="disabled" />'
              : '<input type="checkbox" disabled="disabled" />';
          },
        },
      ],
      pageLength: 5,
    });
  }

  gamesTable = $("#gamesTable").DataTable({
    ajax: {
      //url: "api/gamesData.json",
      dataSrc: "",
    },
    columns: [
      { data: "gameID", title: "Game ID" },
      { data: "title", title: "Title" },
      { data: "numberOfDownloads", title: "Number of Downloads" },
      { data: "totalRevenue", title: "Total Revenue ($)" },
    ],
    pageLength: 5,
  });

  // Hide games table initially
  $("#gamesTable_wrapper").hide();

  // Switch between tables
  $("#tableSwitcher").on("click", function () {
    if (currentTable === "users") {
      $("#usersTable_wrapper").hide();
      $("#gamesTable_wrapper").show();
      currentTable = "games";
    } else {
      $("#gamesTable_wrapper").hide();
      $("#usersTable_wrapper").show();
      currentTable = "users";
    }
  });

  // Handle search functionality
  $("#searchInput").on("keyup", function () {
    if (currentTable === "users") {
      usersTable.search(this.value).draw();
    } else {
      gamesTable.search(this.value).draw();
    }
  });

  // Handle row display length change
  $("#rowCount").on("change", function () {
    if (currentTable === "users") {
      usersTable.page.len(this.value).draw();
    } else {
      gamesTable.page.len(this.value).draw();
    }
  });

  // Handle pagination
  $("#prevPage").on("click", function () {
    if (currentTable === "users") {
      usersTable.page("previous").draw("page");
    } else {
      gamesTable.page("previous").draw("page");
    }
  });

  $("#nextPage").on("click", function () {
    if (currentTable === "users") {
      usersTable.page("next").draw("page");
    } else {
      gamesTable.page("next").draw("page");
    }
  });

  // Handle active status checkbox change
  $(document).on("change", ".active-checkbox", function () {
    let rowData = $(this).closest("tr").data();
    rowData.active = this.checked;
    // Here you can add an AJAX call to update the active status on the server
  });
});
