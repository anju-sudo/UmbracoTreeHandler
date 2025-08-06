angular.module("umbraco").controller("multiCheckboxDropdownDefaultController", function ($scope) {
    $scope.dropdownOpen = false;
    $scope.availableOptions = [];

    // Initialize model value
    if (!$scope.model.value) {
        $scope.model.value = [];
    } else if (!Array.isArray($scope.model.value)) {
        $scope.model.value = [$scope.model.value];
    }

    // Get options from prevalues/configuration
    if ($scope.model.config && $scope.model.config.options) {
        // Handle different format possibilities
        if (Array.isArray($scope.model.config.options)) {
            $scope.availableOptions = $scope.model.config.options;
        } else if (typeof $scope.model.config.options === 'string') {
            try {
                $scope.availableOptions = JSON.parse($scope.model.config.options);
            } catch (e) {
                $scope.availableOptions = $scope.model.config.options.split(',').map(function (item) {
                    var parts = item.trim().split('|');
                    return {
                        value: parts[0].trim(),
                        label: parts[1] ? parts[1].trim() : parts[0].trim()
                    };
                });
            }
        }
    } else {
        $scope.availableOptions = [
            { value: 'option1', label: 'Option 1' },
            { value: 'option2', label: 'Option 2' },
            { value: 'option3', label: 'Option 3' }
        ];
    }

    // **NEW CODE: Clean up orphaned values**
    $scope.cleanupOrphanedValues = function () {
        var validValues = $scope.availableOptions.map(function (option) {
            return option.value;
        });

        $scope.model.value = $scope.model.value.filter(function (value) {
            return validValues.indexOf(value) !== -1;
        });
    };

    // Call cleanup when options are loaded
    $scope.cleanupOrphanedValues();

    $scope.toggleOption = function (value) {
        const idx = $scope.model.value.indexOf(value);
        if (idx > -1) {
            $scope.model.value.splice(idx, 1);
        } else {
            $scope.model.value.push(value);
        }
    };

    $scope.toggleDropdown = function () {
        $scope.dropdownOpen = !$scope.dropdownOpen;
    };

    $scope.getSelectedLabels = function () {
        return $scope.availableOptions
            .filter(option => $scope.model.value.includes(option.value))
            .map(option => option.label);
    };

    $scope.getDisplayText = function () {
        var selectedLabels = $scope.getSelectedLabels();
        return selectedLabels.length ? selectedLabels.join(', ') : '- Select Options -';
    };

    // Watch for changes to available options and clean up orphaned values
    $scope.$watch('availableOptions', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.cleanupOrphanedValues();
        }
    }, true);
});
