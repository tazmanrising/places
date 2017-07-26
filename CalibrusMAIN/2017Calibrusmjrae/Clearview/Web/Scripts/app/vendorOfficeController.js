(function () {

	'use strict';

	angular.module("portal")
        .controller("vendorOfficeController", vendorOfficeController);

	function vendorOfficeController(portalData, $log, $location, $filter, $uibModal) {

		var vm = this;

		vm.pageChanged = function () {
			$log.info("current page: " + vm.currentPage);
			var start = (vm.currentPage - 1) * vm.pageSize;
			var end = start + vm.pageSize;
			vm.officePage = vm.filter.length > 0 ? vm.filteredOffices.slice(start, end) : vm.offices.slice(start, end);
		}

		var getoffices = function () {
			$log.info("Getting offiecs for refresh");
			portalData.getVendorOffices(vm.vendorId, false).then(onComplete, onError);
		};

		var onComplete = function (data) {
			vm.offices = data;
			vm.totalItems = data.length;
			vm.officePage = vm.offices.slice(0, vm.pageSize);
			vm.currentPage = 1;
		};

		var onError = function (reason) {
			vm.error = "Error getting office list";
		};

		var onStatusUpdate = function (reason) {
			$log.info("onStatusUpdate: success");
			$log.info("onStatusUpdate: Before Getting offices for refresh");
			getoffices();
			$log.info("onStatusUpdate: After Getting offices for refresh");

			if (vm.filter.length > 0) {
				$log.info("onStatusUpdate: filter offices for refresh");
				$log.info("onStatusUpdate: filter = " + vm.filter);
				vm.filterOffices();
			}

		};

		var onStatusUpdateError = function (reason) {
			vm.error = "Error updating office status";
		};

		vm.filterOffices = function () {
			$log.info('filterOffices: ' + vm.filter);
			vm.filteredOffices = $filter('filter')(vm.offices, function (item, index) {
			    if (item.OfficeName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.OfficeEmail.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) {
					$log.info('filterOffices $filter: ' + item.LastName);
					return true;
				}
				return false;
			});
			$log.info('vm.filteredOffices.length : ' + vm.filteredOffices.length);
			vm.totalItems = vm.filteredOffices.length;
			vm.officePage = vm.filteredOffices.slice(0, vm.pageSize);
			vm.currentPage = 1;
		};

		vm.open = function (u) {

			$log.info('open');
			$log.info(u);

			var modalInstance = $uibModal.open({
				templateUrl: 'myModalContent.html',
				controller: 'officeLogController',
				controllerAs: 'pop',
				backdrop: 'static',
				resolve: {
					office: function () {
						return u;
					}
				}
			});

			modalInstance.result.then(function (data) {
				$log.info("modalInstance.result officeId: " + data.officeId);
				$log.info("modalInstance.result loggedInUser: " + data.loggedInUser);
				portalData.updateOfficeStatus(data.officeId, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);



			}, function () {
				$log.info('Modal dismissed at: ' + new Date());
			});
		};

		vm.vendorId = $location.absUrl().split('/');
		vm.vendorId = vm.vendorId.pop();
		getoffices(vm.vendorId);

		vm.offices;
		vm.officePage;
		vm.currentPage;


		vm.pageSize = 10;


		vm.totalItems;
		vm.filter = "";

	}

}());

(function () {

	'use strict';

	angular.module("portal")
        .controller("officeLogController", officeLogController);

	function officeLogController(portalData, $log, $uibModalInstance, office) {

		var pop = this;

		pop.office = office;

		pop.header = office.IsActive ? 'Inactivate ' : 'Reactivate ';
		pop.header += office.OfficeName + '?';

		pop.ok = function (u) {

			$uibModalInstance.close({ officeId: office.Id, loggedInUser: u });
		};

		pop.cancel = function () {
			$uibModalInstance.dismiss('cancel');
		};

	}

}());