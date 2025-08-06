angular.module("umbraco").controller("multiCheckboxPrevaluesController", function ($scope) {

    // Initialize the model value as an array of option objects
    if (!$scope.model.value) {
        $scope.model.value = [
            { value: 'Value 1', label: 'Label 1' },
            { value: 'Value 2', label: 'Label 2' }
        ];
    }

    // If model.value is a string (from old format), convert it
    if (typeof $scope.model.value === 'string') {
        try {
            var parsed = JSON.parse($scope.model.value);
            $scope.model.value = parsed;
        } catch (e) {
            // If it's not JSON, try to parse as comma-separated values
            $scope.model.value = $scope.model.value.split(',').map(function(item) {
                var parts = item.trim().split('|');
                return {
                    value: parts[0].trim(),
                    label: parts[1] ? parts[1].trim() : parts[0].trim()
                };
            });
        }
    }

    // Ensure we have at least one option
    if (!Array.isArray($scope.model.value) || $scope.model.value.length === 0) {
        $scope.model.value = [
            { value: 'option1', label: 'Option 1' }
        ];
    }

    $scope.addOption = function() {
        var newIndex = $scope.model.value.length + 1;
        $scope.model.value.push({
            value: 'option' + newIndex,
            label: 'Option ' + newIndex
        });
    };

    $scope.removeOption = function(index) {
        $scope.model.value.splice(index, 1);
        
        // If no options left, add a default one
        if ($scope.model.value.length === 0) {
            $scope.model.value.push({
                value: 'option1',
                label: 'Option 1'
            });
        }
    };

    // Watch for changes and ensure we always have valid data
    $scope.$watch('model.value', function(newVal) {
        if (newVal && Array.isArray(newVal)) {
            // Clean up any empty values
            newVal.forEach(function(option) {
                if (!option.value) {
                    option.value = '';
                }
                if (!option.label) {
                    option.label = option.value || '';
                }
            });
        }
    }, true);
});