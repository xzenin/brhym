 var app = angular.module('rulemap', []);
 app.controller('RuleController', function RuleController($scope, $http, $filter) {
     $scope.domain = window.domain || "";
     $scope.aset = window.aset;
     $scope.title = "SQUAD";
     $scope.subtitle = "Task Manager";

     $scope.sid = window.token.sid;

     $scope.message = "loaded!";
     $scope.predicate = 'none';

     $scope.sort = function (predicate) {
         $scope.predicate = predicate;
     }

     $scope.isSorted = function (predicate) {
         return ($scope.predicate == predicate)
     }

     $scope.edititem = {}
     $scope.isdirty =false;
     $scope.idcol = "id";
     $scope.edit = function(item, idcol)
     {
        $scope.edititem = JSON.parse(JSON.stringify( item));
        $scope.idcol = idcol;
        showConfiguration();
     };
     $scope.save = function()
     {
        var rows = $scope.grid.rows.filter(x=> x[ $scope.idcol] !==  $scope.edititem[ $scope.idcol]);
        rows.push($scope.edititem);
        $scope.grid.search(rows);
        hideConfiguration();
     };

     $scope.remove = function(item,idcol)
     {
        var sure = window.confirm("Are you sure to delete this item ?");
        if(sure)
        {
           var rows = $scope.grid.rows.filter(x=> x[idcol] !== item[idcol]);
           $scope.grid.search(rows);
        }
     };

     ///////////////////////////////////

     $scope.grid = {};

     $scope.grid.sort = {
         sortingOrder: '',
         reverse: false
     };

     $scope.grid.gap = 5;

     $scope.grid.filteredItems = [];
     $scope.grid.groupedItems = [];
     $scope.grid.itemsPerPage = 10;
     $scope.grid.pagedItems = [];
     $scope.grid.currentPage = 0;


     var searchMatch = function (haystack, needle) {
         if (!needle) {
             return true;
         }
         return haystack.toLowerCase().indexOf(needle.toLowerCase()) !== -1;
     };

     $scope.grid.pagechange = function () {
         $scope.grid.search($scope.grid.rows);
     };
     // init the filtered items
     $scope.grid.search = function (rows) {

         $scope.grid.rows = rows;

         $scope.grid.filteredItems = $filter('filter')(rows, function (item) {

             for (var attr in item) {
                 if (searchMatch(item[attr], $scope.grid.query))
                     return true;
             }

             return false;
         });

         // take care of the sorting order
         if ($scope.grid.sort.sortingOrder !== '') {
             $scope.grid.filteredItems = $filter('orderBy')($scope.grid.filteredItems, $scope.grid.sort.sortingOrder, $scope.grid.sort.reverse);
         }

         $scope.grid.currentPage = 0;
         // now group by pages
         $scope.grid.groupToPages();
     };


     // calculate page in place
     $scope.grid.groupToPages = function () {
         $scope.grid.pagedItems = [];

         for (var i = 0; i < $scope.grid.filteredItems.length; i++) {
             if (i % $scope.grid.itemsPerPage === 0) {
                 $scope.grid.pagedItems[Math.floor(i / $scope.grid.itemsPerPage)] = [$scope.grid.filteredItems[i]];
             } else {
                 $scope.grid.pagedItems[Math.floor(i / $scope.grid.itemsPerPage)].push($scope.grid.filteredItems[i]);
             }
         }
     };

     $scope.grid.range = function (size, start, end) {
         if (size < 13) {
             size = 13;
         }
         var ret = [];

         if (size < end) {
             end = size;
             start = size - $scope.grid.gap;
         }
         for (var i = start; i < end; i++) {
             ret.push(i);
         }
         return ret;
     };

     $scope.grid.prevPage = function () {
         if ($scope.grid.currentPage > 0) {
             $scope.grid.currentPage--;
         }
     };

     $scope.grid.nextPage = function () {
         if ($scope.grid.currentPage < $scope.grid.pagedItems.length - 1) {
             $scope.grid.currentPage++;
         }
     };

     $scope.grid.setPage = function (n) {
         if (n > $scope.grid.pagedItems.length - 1) {} else {
             $scope.grid.currentPage = n;
         }
     };

     $scope.grid.sort_by = function (newSortingOrder) {

         if ($scope.grid.sort.sortingOrder == newSortingOrder) {
             $scope.grid.sort.reverse = !$scope.grid.sort.reverse;
         }
         $scope.grid.sort.sortingOrder = newSortingOrder

         // call search again to make sort affect whole query
         $scope.grid.search($scope.grid.rows);
     };

     /////////////////////////////////////////

     $scope.model = {};
     $scope.model.visiblecolumns = [];

     $scope.hiddencolumns = ['days', 'preparationDate', 'sl', 'dirty', 'bucketDate', 'exitDate', '$$hashkey'];
     $scope.headoptions = {};
     $scope.columns = [];
     $scope.makeoptions = function (moves) {
         if (moves.length <= 0)
             return $scope.headoptions;

         var aline = moves[0];
         $scope.headoptions = {};
         $scope.columns = Object.keys(aline);
         for (k = 0; k < $scope.columns.length; k++) {
             var key = $scope.columns[k];
             $scope.headoptions[key] = distinct(moves, key);
         }
         //-ToDo
         $scope.model.visiblecolumns = [];
         $scope.columns.forEach(x => {
             var col = {
                 name: x,
                 visible: !$scope.hiddencolumns.includes(x)
             };
             $scope.model.visiblecolumns.push(col);
         });
     };

     function distinct(array, key) {
         var ta = [];
         for (i = 0; i < array.length; i++) {
             var line = array[i];
             var dc = line[key];
             if (ta.indexOf(dc) == -1) {
                 ta.push(dc);
             }
         }
         return ta;
     }

     $scope.load = function () {
         var sidWithExtra = $scope.sid;
         var envgeturl = $scope.domain + "/db/moves?dbfile=" + sidWithExtra + "&reset=false";
         $scope.loading = true;
         $http.get(envgeturl)
             .then(function (result) {
                 console.log(result.data);
                 $scope.model = result.data.model;
                 $scope.makeoptions($scope.model.moves);
                 $scope.grid.cols = $scope.model.visiblecolumns;
                 $scope.grid.search($scope.model.moves);
               
                 $scope.grid.sort.sortingOrder = '';
                 $scope.loading = false;

                 showChart(result.data);
                $scope.loading = false;
             }, function (result) {
                 $scope.loading = false;
                 alert("error");
                 console.log(result);
             });
     };

     $scope.readAll = function () {

        var sidWithExtra = $scope.sid;
        var envgeturl = $scope.domain + "/db/readall?dbfile=" + sidWithExtra + "&reset=true";
        $scope.loading = true;
        $http.get(envgeturl)
            .then(function (result) {
                console.log(result.data);
                $scope.model = result.data.model;
                $scope.makeoptions($scope.model.moves);
                $scope.grid.search($scope.model.moves);
                $scope.grid.cols = $scope.model.visiblecolumns;
                $scope.grid.sort.sortingOrder = '';
              

                $(document).find(".calendar").datepicker();
                $scope.loading = false;
            }, function (result) {
                $scope.loading = false;
                alert("error");
                console.log(result);
            });
    };

     $scope.startOver = function () {

         var sidWithExtra = $scope.sid;
         var envgeturl = $scope.domain + "/db/moves?dbfile=" + sidWithExtra + "&reset=true";
         $scope.loading = true;
         $http.get(envgeturl)
             .then(function (result) {
                 console.log(result.data);
                 $scope.model = result.data.model;
                 $scope.makeoptions($scope.model.moves);
                 $scope.grid.search($scope.model.moves);
                 $scope.grid.cols = $scope.model.visiblecolumns;
                 $scope.grid.sort.sortingOrder = '';
               

                 $(document).find(".calendar").datepicker();
                 $scope.loading = false;
             }, function (result) {
                 $scope.loading = false;
                 alert("error");
                 console.log(result);
             });
     };

     $scope.fireRules = function () {

         var valid = $scope.validateRules();
         if (!valid) {
             alert("Some invalid data.");
             return false;
         }
         var sidWithExtra = $scope.sid;
         var envgeturl = $scope.domain + "/db/firerules";
         var rulemodel = {
             ruleset: $scope.ruleset,
             token: window.token,
             dbfile: sidWithExtra,
             bucketsize: 5,
             bucketcounter: 0
         };
         $scope.ruleprogress = 'loading';
         $http.post(envgeturl, {
                 rulemodel: rulemodel
             })
             .then(function (result) {
                 log("Successfully run rules.");
                 log(result);
                 $scope.model = result.data.model;
                 $scope.makeoptions($scope.model.moves);
                 $scope.grid.search($scope.model.moves);
                 $scope.grid.cols = $scope.model.visiblecolumns;
                 $scope.grid.sort.sortingOrder = '';
                 $scope.logs = result.data.logs;
                 hideConfiguration();
                 $scope.ruleprogress = 'done';
             }, function (result) {
                 log("Could not run rules. Details:");
                 log(result);
                 $scope.ruleprogress = 'error';
                 //alert("Error in posting...");
                 console.log(result);

             });
     };

     $scope.addExtraColumn = function (op) {

         var col = $scope.extracolumn; 
         if (!col) {
             alert("Column value blank.");
             return false;
         }
         var sidWithExtra = $scope.sid;
         var envgeturl = $scope.domain + "/db/addcolumn";
         var rulemodel = {
             token: window.token,
             dbfile: sidWithExtra,
             colname: col,
             op: op
         };
         $scope.ruleprogress = 'loading';
         $http.post(envgeturl, {
                 rulemodel: rulemodel
             })
             .then(function (result) {
                 log("Successfully added columns.");
                 log(result);
                 $scope.model = result.data.model;
                 $scope.makeoptions($scope.model.moves);
                 $scope.grid.search($scope.model.moves);
                 $scope.grid.cols = $scope.model.visiblecolumns;
                 $scope.grid.sort.sortingOrder = '';
                 $scope.logs = result.data.logs;
                 hideColumns();
                 $scope.ruleprogress = 'done';
             }, function (result) {
                 log("Could not add column. Details:");
                 log(result);
                 $scope.ruleprogress = 'error';
                 //alert("Error in posting...");
                 console.log(result);

             });
     };
     $scope.validateRules = function () {
        var result = true;
        $scope.ruleset.mg_rules.forEach(x => {
            if (!x.given) {
                result = false;
            }
        });
        return result;
    }
     $scope.sortConfigurationSave = function (sjson) {
         var newSortOrder = [];
         for (i = 0; i < $scope.rules.length; i++) {
             newSortOrder.push($scope.rules[i].id);
         }
         alert(newSortOrder);
     };

     //////////////////////////////////////////////////////////

     $scope.ruleset = {
         mg_rules: [],
         env_rules: [],
         dc_date_rules: [],
         order_rules: [],
         scheduling_rules: [],
     };
     $scope.addUpdateRule = function (field, args) {
         var name = "name_" + Math.random();
         var id = "h_" + Math.random();
         var mgrule = {
             sl: $scope.ruleset.env_rules.length + 1,
             id: id,
             template: "update",
             type: "string",
             name: name,
             avalue: "",
             rules: [{
                 field: field,
                 value: "",
                 sign: "equals",
                 op: "AND"

             }]
         }
         $scope.ruleset.env_rules.push(mgrule);
         $scope.message = "Changed MG data...";
     };
     $scope.addMGRule = function (field, args) {
         var name = "name_" + Math.random();
         var id = "h_" + Math.random();
         var mgrule = {
             sl: $scope.ruleset.mg_rules.length + 1,
             id: id,
             template: "constraint",
             type: "days",
             name: name,
             avalue: "",
             rules: [{
                 field: field,
                 value: "",
                 sign: "equals",
                 op: "AND"

             }]
         }
         $scope.ruleset.mg_rules.push(mgrule);
         $scope.message = "Changed MG data...";
     };
     $scope.getoptions = function (key) {
         return $scope.headoptions[key];
     };

     $scope.addmg = function (rin, rl, n) {
         rin.rules.push({
             field: rl.field,
             value: rl.pvalue,
             sign: rl.sign,
             op: rl.op
         });

     };

     $scope.removemg = function (rin, rl, n) {
         var index = rin.rules.indexOf(rl);
         if (index > -1) {
             rin.rules.splice(index, 1);
         }
     }

     $scope.removerule = function (rules, rin) {
         var index = rules.indexOf(rin);
         if (index > -1) {
             rules = rules.splice(index, 1);
         }
     }

     $scope.addOrderRule = function (field) {
         $scope.ruleset.order_rules.push({
             sl: $scope.ruleset.order_rules.length + 1,
             template: "order",
             id: "h_" + Math.random(),
             name: field,
             pvalue: "",
             type: "string",
             direction: "asc",
             order: $scope.ruleset.order_rules.length + 1
         });
         $scope.message = "Changed ORDER data...";
     };
     $scope.addScheduleRule = function (field) {
         switch (field) {
             case "bucket":
                 $scope.ruleset.scheduling_rules.push({
                     sl: $scope.ruleset.scheduling_rules.length + 1,
                     template: "scheduling",
                     id: "h_" + Math.random(),
                     name: field,
                     type: "string",
                     classifier: "Week",
                     duration: "4"
                 });
                 break;
             case "start":
                 $scope.ruleset.scheduling_rules.push({
                     sl: $scope.ruleset.scheduling_rules.length + 1,
                     template: "scheduling",
                     id: "h_" + Math.random(),
                     name: field,
                     type: "date",
                     classifier: "StartDate",
                     duration: "12-12-2018"
                 });
                 break;
         }
         $scope.message = "Changed SCHEDULE data...";
     };


     $scope.removeMGInstance = function (rin) {
         var index = $scope.ruleset.mg_rules.indexOf(rin);
         if (index > -1) {
             $scope.ruleset.mg_rules.splice(index, 1);
         }
     }
     $scope.removeOrderInstance = function (rin) {
         var index = $scope.ruleset.order_rules.indexOf(rin);
         if (index > -1) {
             $scope.ruleset.order_rules.splice(index, 1);
         }
     }
     $scope.removeSchedulingInstance = function (rin) {
         var index = $scope.ruleset.scheduling_rules.indexOf(rin);
         if (index > -1) {
             $scope.ruleset.scheduling_rules.splice(index, 1);
         }
     }

     var deleteOne = function (array, attr, value) {
         var index = getIndexIfObjWithAttr(array, attr, value);
         array.splice(index, 1);
     };
     var getIndexIfObjWithAttr = function (array, attr, value) {
         for (var i = 0; i < array.length; i++) {
             if (array[i][attr] === value) {
                 return i;
             }
         }
         return -1;
     }


     $scope.load();


 });

 app.$inject = ['$scope', '$filter'];

 app.directive("customSort", function () {
     return {
         restrict: 'A',
         transclude: true,
         scope: {
             order: '=',
             sort: '='
         },
         template: ' <a ng-click="sort_by(order)" style="color: #555555;">' +
             '    <span ng-transclude></span>' +
             '    <i ng-class="selectedCls(order)"></i>' +
             '</a>',
         link: function (scope) {

             // change sorting order
             scope.sort_by = function (newSortingOrder) {
                 var sort = scope.sort;

                 if (sort.sortingOrder == newSortingOrder) {
                     sort.reverse = !sort.reverse;
                 }

                 sort.sortingOrder = newSortingOrder;
             };


             scope.selectedCls = function (column) {
                 if (column == scope.sort.sortingOrder) {
                     return ('icon-chevron-' + ((scope.sort.reverse) ? 'down' : 'up'));
                 } else {
                     return 'icon-sort'
                 }
             };
         } // end link
     }
 });