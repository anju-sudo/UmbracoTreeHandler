
# UmbracoTreeHandler

A powerful utility package for Umbraco 13 that extends content and media visibility by enabling user group-based access control, along with a custom multi checkbox dropdown field for flexible data configuration.

### Features
User Group-Based Content & Media Visibility
Show or hide content/media nodes in the Umbraco tree based on user groups.

Custom Multi Checkbox Dropdown Default
A manually configurable multi-checkbox dropdown field for datatypes. Ideal when you need full control over the dropdown values.

### Installation

#### Option 1: NuGet
Install-Package UmbracoTreeHandler

#### Option 2: Manual
Download the package .zip or .dll
Place it in your /bin folder and restart the site

### How to Use

1. Restrict Tree Visibility by User Group

Once installed, the package will restrict content and media visibility in the Umbraco backoffice tree based on the logged-in user's group
Non-admin users cannot able to edit the multi checkbox dropdown(That populate usergroups). They can use multi checkbox dropdown default.
Read the full guide on implementation and configuration here:
https://umbracotreehandler.blogspot.com/

2. Multi Checkbox Dropdown Default (Custom)

In addition to the main functionality, this package includes a  Multi Checkbox Dropdown Default.

#### Why use it?
Umbraco's native Checkbox Dropdown automatically fetches and displays user groups.

If you need a static/custom list of values that you define manually, use 'Multi Checkbox Dropdown Default'.

#### Where to find it:
Navigate to Settings > Data Types

Create a new datatype and choose Checkbox Dropdown Default

Define your own list of label-value pairs