angular.module("umbraco").controller("multiCheckboxDropdownController", function ($scope, $http) {

    $scope.dropdownOpen = false;
    $scope.userGroups = [];
    $scope.isAdmin = false;
    $scope.isLoading = true;

    if (!$scope.model.value) {
        $scope.model.value = [];
    } else if (!Array.isArray($scope.model.value)) {
        $scope.model.value = [$scope.model.value];
    }

    // Check if current user is admin
    $http.get("/umbraco/backoffice/api/UserGroupPicker/CheckAdminAccess")
        .then(function (response) {
            console.log("Admin access check:", response.data);
            $scope.isAdmin = response.data.isAdmin;
            $scope.isLoading = false;

            // If not admin, close dropdown and prevent interaction
            if (!$scope.isAdmin) {
                $scope.dropdownOpen = false;
            }
        })
        .catch(function (error) {
            console.error("Error checking admin access:", error);
            $scope.isAdmin = false;
            $scope.isLoading = false;
        });

    // Load user groups
    $http.get("/umbraco/backoffice/api/UserGroupPicker/GetAll")
        .then(function (response) {
            console.log("User groups loaded:", response.data);

            $scope.userGroups = response.data.map(function (group) {
                return {
                    alias: group.alias,
                    name: group.name
                };
            });
        })
        .catch(function (error) {
            console.error("Error loading user groups:", error);
        });

    $scope.toggleGroup = function (alias) {
        // Only allow changes if user is admin
        if (!$scope.isAdmin) {
            return;
        }

        const idx = $scope.model.value.indexOf(alias);
        if (idx > -1) {
            $scope.model.value.splice(idx, 1);
        } else {
            $scope.model.value.push(alias);
        }
    };

    $scope.toggleDropdown = function () {
        // Only allow dropdown toggle if user is admin
        if (!$scope.isAdmin) {
            return;
        }

        $scope.dropdownOpen = !$scope.dropdownOpen;
    };

    $scope.getSelectedLabels = function () {
        return $scope.userGroups
            .filter(group => $scope.model.value.includes(group.alias))
            .map(group => group.name);
    };

    $scope.getDisplayText = function () {
        if ($scope.isLoading) {
            return 'Loading...';
        }

        if (!$scope.isAdmin) {
            return 'User Group Access Control';
        }

        var selectedLabels = $scope.getSelectedLabels();
        return selectedLabels.length ? selectedLabels.join(', ') : '- Select User Groups -';
    };
});