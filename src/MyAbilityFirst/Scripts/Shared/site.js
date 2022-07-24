﻿var ajaxDataObj = (
	function () {
		var data;
		var pub = {};

		pub.getData = function () {
			return data;
		};
		pub.setData = function (newData) {
			data = newData;
		};
		pub.flipSelected = function () {
			data.Selected = !data.Selected;
		};
		return pub;
	}()
);

function AJAXPostReplace(url, data, buttonElement, replaceElement, followUp) {
	followUp.unshift(ReplaceCallback(buttonElement, replaceElement));
	AJAXPost(url, data, followUp, WriteErrorToConsole)
};

function AJAXPostShortlistButton(url, data, buttonElement, toggleClasses, followUp) {
	if (ajaxDataObj.getData() == null) {
		ajaxDataObj.setData(data);
	}
	ajaxDataObj.flipSelected();

	followUp.unshift(ToggleButtonCallback(buttonElement, toggleClasses))
	$(buttonElement).button('loading').delay(100).queue(function () {
		AJAXPost(url, ajaxDataObj.getData(), followUp, ResetButtonCallback(buttonElement))
	});
};


function AJAXPost(url, data, successCallback, failCallback) {
	$.ajax({
		data: JSON.stringify(data),
		contentType: "application/json; charset=utf-8",
		type: 'POST',
		cache: false,
		url: url,
		timeout: 5000,
		success: function (datares) {
			for (var i = 0; i < successCallback.length; i++) {
				successCallback[i](datares);
			}
		}
	}).fail(function (errorThrown) {
		failCallback(errorThrown)
	});
}

function ToggleButtonCallback(buttonElement, toggleClasses) {
	return function (datares) {
		$(buttonElement).toggleClass(toggleClasses);
		ResetDequeueButton(buttonElement);
		ajaxDataObj.setData(datares);
	}
}

function ResetButtonCallback(buttonElement) {
	return function (errorThrown) {
		ajaxDataObj.flipSelected();
		ResetDequeueButton(buttonElement);
		WriteErrorToConsole(errorThrown);
	}
}

function ReplaceCallback(buttonElement, divReplaceElement) {
	return function (datares) {
		$(divReplaceElement).replaceWith(datares);
		ResetDequeueButton(buttonElement);
	}
}

function ResetDequeueButton(buttonElement) {
	$(buttonElement).button('reset');
	$(buttonElement).dequeue();
}

function WriteErrorToConsole(errorThrown) {
	console.log(errorThrown.statusText);
}

function GetCheckBoxListSelectedValues(checkBoxName) {
	var chkBox = document.querySelectorAll("input[name='" + checkBoxName + "'][type='checkbox']");
	var checkeditems = [];
	for (var i = 0; i < chkBox.length; i++) {
		if (chkBox[i].checked) {
			checkeditems.push(chkBox[i].value);
		}
	}
	return JSON.stringify(checkeditems).replace(/['"]+/g, '');
}

function GetAutocompleteCategories() {
	var allSubcategoryCheckboxes = document.querySelectorAll("input[type='checkbox']");
	var allSubcategoryNames = [];
	var data = new Object();
	for (var i = 0; i < allSubcategoryCheckboxes.length; i++) {
		allSubcategoryNames.push({
			label: allSubcategoryCheckboxes[i].parentNode.childNodes[1].innerText,
			category: allSubcategoryCheckboxes[i].parentNode.parentNode.parentNode.parentNode.getAttribute('label'),
			categoryID: allSubcategoryCheckboxes[i].value
		});
	}
	return allSubcategoryNames;
}

// Returns an array of [Lat,Long,DisplayingHTML]
function GetMarkerInfoFromDOM(name, displayHtmlType) {
	var allLat = document.querySelectorAll("input[name^='" + name + "['][name$='].Latitude']");
	var allLng = document.querySelectorAll("input[name^='" + name + "['][name$='].Longitude']");
	var allHtml = document.querySelectorAll(displayHtmlType + "[name^='" + name + "['][name$='].Html']");
	var count = allLat.length;

	var res = [];
	for (var i = 0; i < count; i++) {
		var item = [];
		item.push(allLat[i].value);
		item.push(allLng[i].value);
		item.push(allHtml[i].innerHTML);
		res.push(item);
	}
	return res;
}

var rad = function (x) {
	return x * Math.PI / 180;
};

var getDistance = function (p1, p2) {
	var R = 6378100; // Earth’s mean radius in meter
	var dLat = rad(p2.lat() - p1.lat());
	var dLong = rad(p2.lng() - p1.lng());
	var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(rad(p1.lat())) * Math.cos(rad(p2.lat())) *
    Math.sin(dLong / 2) * Math.sin(dLong / 2);
	var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	var d = R * c;
	return d; // returns the distance in meter
};

function displayDistanceWithUnits(dist) {
	if (dist > 1000) return Math.round(dist) + "km";
	else return Math.round(dist) + "m";
}

var SearchResultsMarkers;
var map;

function initProfileMap() {
	map = new google.maps.Map(document.getElementById('map'), {
		center: { lat: +document.getElementById('HomeLatitude').value, lng: +document.getElementById('HomeLongitude').value },
		mapTypeControl: false,
		streetViewControl: false,
		zoom: 11
	});

	var marker = new google.maps.Marker({
		position: map.center,
		map: map,
		title: 'Click me to search within area'
	});
	marker.setMap(map);

	var circle = new google.maps.Circle({
		strokeColor: '#FF00FF',
		strokeOpacity: 0.8,
		strokeWeight: 2,
		fillColor: '#FFFFFF',
		fillOpacity: 0.30,
		center: marker.position,
		radius: +document.getElementById('LocationCoverageRadius').value * 1000,
		draggable: false
	});
	circle.setMap(map)

}

function initSearchMap() {

	var lat = +document.getElementById('Latitude').value;
	if (lat == 0) {
		lat = +document.getElementById('HomeLatitude').value;
	}
	var lng = +document.getElementById('Longitude').value;
	if (lng == 0) {
		lng = +document.getElementById('HomeLongitude').value;
	}

	map = new google.maps.Map(document.getElementById('map'), {
		center: { lat: lat, lng: lng },
		mapTypeControl: false,
		streetViewControl: false,
		zoom: 12
	});

	var marker = new google.maps.Marker({
		position: map.center,
		map: map,
		title: 'Click me to search within area'
	});
	marker.setMap(map);

	var circle = new google.maps.Circle({
		strokeColor: '#FF00FF',
		strokeOpacity: 0.8,
		strokeWeight: 2,
		fillColor: '#FFFFFF',
		fillOpacity: 0.30,
		center: marker.position,
		radius: 5000,
		draggable: true
	});

	circle.addListener('click', function (event) {
		newLocationClicked(marker, circle, event.latLng);
	});

	circle.addListener('drag', function (event) {
		circle.center = marker.position;
		circle.radius = getDistance(event.latLng, circle.center);
	});

	circle.addListener('dragend', function (event) {
		circle.center = marker.position;
		circle.draggable = false;
		newLocationClicked(marker, circle, marker.position);
		circle.draggable = true;
	});

	marker.addListener('click', function () {
		if (circle.map == null) {
			circle.setMap(map);
			document.getElementById('RadiusInKm').value = circle.radius / 1000;
		}
		else {
			circle.setMap(null);
			document.getElementById('RadiusInKm').value = null;
		}
		var markerUpdateFunctions = [];
		markerUpdateFunctions.push(populateMarkers(map));
		searchResults(1, markerUpdateFunctions);
	});

	SearchResultsMarkers = [];
	populateMarkers(null)();

	google.maps.event.addListener(map, 'click', function (event) {
		newLocationClicked(marker, circle, event.latLng);
	});
}

function newLocationClicked(marker, circle, latLng) {
	map.setCenter(latLng);
	marker.setPosition(latLng);
	document.getElementById('Latitude').value = latLng.lat();
	document.getElementById('Longitude').value = latLng.lng();
	circle.setCenter(latLng);

	if (circle != null && circle.map != null) {
		document.getElementById('RadiusInKm').value = circle.radius / 1000;
	}
	var markerUpdateFunctions = [];
	markerUpdateFunctions.push(populateMarkers(map));
	searchResults(1, markerUpdateFunctions);
}

function searchResults(pageNumber, followUp) {
	AJAXPostReplace(
		document.getElementById('SearchURL').value,
		{
			SearchTerm: JSON.stringify(document.getElementsByName('SearchTerm')[0].value).replace(/['"]+/g, ''),
			Longitude: JSON.stringify(document.getElementById('Longitude').value).replace(/['"]+/g, ''),
			Latitude: JSON.stringify(document.getElementById('Latitude').value).replace(/['"]+/g, ''),
			RadiusInKm: JSON.stringify(document.getElementById('RadiusInKm').value).replace(/['"]+/g, ''),
			PostedSubcategoryIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedSubCategoryIDs')),
			PostedGenderIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedGenderIDs')),
			PostedLanguageIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedLanguageIDs')),
			PostedCultureIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedCultureIDs')),
			PostedReligionIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedReligionIDs')),
			PostedPersonalityIDs: JSON.parse(GetCheckBoxListSelectedValues('PostedPersonalityIDs')),
			PageNumber: pageNumber,
			SortOption: jQuery("#SortOption option:selected").val()
		},
		'',
		'#div-search-results',
		followUp
	)
}

function populateMarkers() {
	return function (datares) {
		// deallocate existing markers
		for (var i = 0; i < SearchResultsMarkers.length; i++) {
			SearchResultsMarkers[i].infoWindow.setMap(null);
			SearchResultsMarkers[i].infoWindow = null;
			SearchResultsMarkers[i].setMap(null);
			delete SearchResultsMarkers[i];
		};
		SearchResultsMarkers.length = 0;

		// create new markers
		var newMarkers = GetMarkerInfoFromDOM("SearchResults", "div");
		newMarkers.forEach(function (item) {
			var content = item[2];
			var infoWindow = new google.maps.InfoWindow({
				content: content
			});

			var newMarker = new google.maps.Marker({
				position: new google.maps.LatLng(item[0], item[1]),
				map: map,
				infoWindow: infoWindow
			});

			newMarker.addListener('mouseover', function () {
				infoWindow.open(map, this);
			});

			newMarker.addListener('mouseout', function () {
				infoWindow.close();
			});

			SearchResultsMarkers.push(newMarker);
		});
	};
}

$('.category-tabs').click(function () {
	$(this).find('i').toggleClass('fa-angle-double-right fa-angle-double-down');
});


$(function () {
	// AutoComplete with categories widget
	$.widget("custom.catcomplete", $.ui.autocomplete, {
		_create: function () {
			this._super();
			this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
		},
		_renderMenu: function (ul, items) {
			var that = this,
				currentCategory = "";
			items = items.slice(0,10);
			$.each(items, function (index, item) {
				var li;
				if (item.category != currentCategory) {
					ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
					currentCategory = item.category;
				}
				li = that._renderItemData(ul, item);
				if (item.category) {
					li.attr("aria-label", item.category + " : " + item.label);
				}
			});
		}
	});

	// tagsinput
	$('.category-tag-area').tagsinput({
		allowDuplicates: false,
		itemValue: 'id',
		itemText: 'text'
	});

	// initalize tags on page load
	$.each($("input[type='checkbox'][checked='checked']"), function (index, value) {
		$('.category-tag-area').tagsinput('add', { id: value.value, text: value.parentNode.childNodes[1].innerText });
	});

	// AutoComplete on click
	$("#auto-complete-categories").catcomplete({
		delay: 500,
		source: GetAutocompleteCategories(),
		select: function (event, ui) {
			$(".category-tag-area").tagsinput('add', { id: ui.item.categoryID, text: ui.item.label });
			if (ui.item && ui.item.value) {
				ui.item.value = "";
			}
		}
	});
});

// update tags on checkbox change
$('input[type=checkbox]').change(function () {
	if ($(".category-tag-area").length > 0 && this.value != null && this.parentNode.childNodes[1].innerText != null) {
		$(".category-tag-area").tagsinput(this.checked ? 'add' : 'remove', { id: this.value, text: this.parentNode.childNodes[1].innerText });
	}
});

// update checkbox on tags change
$(".category-tag-area").on('itemAdded', function (event) {
	$("input[type='checkbox'][value='" + event.item.id + "']")[0].checked = true;
});

$(".category-tag-area").on('itemRemoved', function (event) {
	$("input[type='checkbox'][value='" + event.item.id + "']")[0].checked = false;
});