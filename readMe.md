[Home]( https://nglthu.github.io/RoadMaps/myRoadMaps/)

[![licence badge]][licence]
[![stars badge]][stars]
[![issues badge]][issues]

[licence badge]:https://img.shields.io/badge/license-MIT-blue.svg
[stars badge]:https://img.shields.io/github/stars/hey-red/Markdown.svg
[issues badge]:https://img.shields.io/github/issues/hey-red/Markdown.svg

[licence]:https://github.com/nglthu/gridDocument/blob/master/LICENSE
[stars]:https://github.com/nglthu/gridDocument/stargazers
[issues]:https://github.com/nglthu/gridDocument/issues



# Grid 
## Assumption and task
I assumed that  documents have been checked by __'s software for duplication, then document and its information will be saved in the cloud database.
Here, as an example, outputs of documents after processing from __'s software are saved in SQL as below: 

![alt text](https://nglthu.github.io/gridDocument/img/db.png)

The task is to create UI as a grid for:
1.  Presenting a list or grid of documents
2.  How a user can search/find documents
3.  Filter by the different duplicate types (identical, near duplicate etc...)
4.  High level info eg. - How many duplicates found in total and for each category
5.  Option to filter by 'confidence score' 0 - 1 (score .95 means we are 95% sure the
files are duplicates)

## Solution

Firstly, ASP.NET MVC C#, Entity Framework, JavaScript, Kendo UI, Linq, HTML are used to achieve the task above. However, this task has not been limited by ASP.NET C# MVC; it can be done by Java if required.

###  Grid of Documents

The input for grid is JSON type. Grid is to read or update data from actions of GetDocumentData and UpdateDocumentData.
```C#
public JsonResult GetDocumentData()
        {
            try
            {

                var documents = db.documents.Include(d => d.duplicateDetail);
                var collection = documents.Select(x => new

                {
                    idDoc = x.idDoc,
                    docName = x.docName,
                    docTypeID = x.docTypeID,
                    note = x.note,
                    confidenceScore=x.confidenceScore

                }).ToList();

                return Json(collection, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = e.ToString() });

            }

        }
And Update documents:
  [HttpPost]

        public JsonResult UpdateDocumentData(document model)
        {
            try
            {
                int Id = Convert.ToInt32(model.idDoc);
                var updateData = (from f in db.documents
                                  where Id == f.idDoc
                                  select f).FirstOrDefault();

                updateData.docName = model.docName;
                updateData.docTypeID = model.docTypeID;
                updateData.note = model.note;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = e.ToString() });
            }

            return Json(model);
        }
The inputs for grid contains fields of idDoc, docTypeID, note, and confidenceScore
The grid view has been coded as below:
@model WebApplication1.Models.document

@{

    ViewBag.Title = "Document List";

}

<legend>Document List</legend>


<div id="documentGrid" style="width: 100%; font-size: 12px">


<script type="text/javascript">

    var duplicateTypeList = @Html.Raw(Json.Encode(ViewBag.duplicateType)) ;

    $(document).ready(function () {

        var grid = $("#documentGrid").kendoGrid({
            selectable: "single row",
            
            allowCopy: true,
            dataSource: new kendo.data.DataSource({
                autoSync: false,
                type: "json",
                transport: {
                    read: {
                        url: "@Url.Action("GetDocumentData", "Home")",
                        dataType: "json"
                    },
                    update: {
                        url: "@Url.Action("UpdateDocumentData", "Home")",
                        dataType: "json",
                        type: "POST",
                        complete: function () {
                            grid.dataSource.read();
                        }
                    },


                },
              
                schema: {
                    errors: "errors",
                    model: {
                        id: "idDoc",
                        fields:
                            {
                                idDoc: { editable: false },
                                docName: {
                                    style: "text-align: center",
                                    type: "string",
                                    validation: { required: true }
                                },
                                docTypeID: {
                                    style: "text-align: center",
                                    type: "number",
                                    validation: { required: true }
                                },
                                note: {
                                    style: "text-align: center",
                                    type: "string",
                                    validation: { required: true }
                                },
                                confidenceScore: {
                                    style: "text-align: center",
                                    type: "number",
                                    validation: { required: true }
                                }
                            }
                    }

                }

            }),
            editable: "popup",
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            reorderable: true,
            groupable: true,
            filterable: true,
            sortable: true,
            //toolbar: ["create"],
            columns: [

                        {field: "docName", title: "Document Name", width: 50},

                        {field: "docTypeID", title: "Duplicate Info", values: duplicateTypeList, 				width: 150, attributes: { "class": "table-cell", style: "text-align: left" }
                         
                        },
                        { field: "confidenceScore", title:"Confidence Score", width: 50 },
                        { field: "note", title: "Note", width: 80 },

             {

                 command: [
                    "edit",
                    {

                        name: "Delete",

                        click: function (e) {

                            e.preventDefault();
                            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
                            window.location.href = "@Url.Action("Delete", "Home")" + "/" + 					dataItem.idDoc;
                        }
                    }
                 ], title: " ", width: 60

             }]

        }).data("kendoGrid");

        $("#documentGrid").kendoTooltip({

            filter: "td:nth-child(5), td:nth-child(7)", //this filter selects the first column cells
            position: "bottom",
            width: 250,
            showAfter: 500,
            content: function (e) {
                var dataItem = $("#documentGrid").data("kendoGrid").dataItem(e.target.closest("tr"));
                var content = "<div style=\"text-align:left\" \"width:250\">" + dataItem.docName;
                return content
            }

        }).data("kendoTooltip");

    });

</script>
```
</div>
### Business Logic

#### How user can search for information:

Every column has fully functioned for filtering and sorting the data, then users can search any information based on its filtering option. 

+ Sorting by double click on every column you want to be sorted. 
+ Filtering by clicking "filter symbol": 

<img src="https://nglthu.github.io/gridDocument/img/filter.png" width="100%" height="100%">

Fig. 1 Grid with supported filtering and sorting.

+ To search any information, you can click filtering in different column and type the value you want to find.
+ Filter by the different duplicate types (identical, near duplicate etc...): you can click in the Duplicate Info, and choose the different duplicate type for filtering.

<img src="https://nglthu.github.io/gridDocument/img/figure2_filterList.png" width="100%" heigh="100%">

Fig. 2 Selected Option of Filtering by the different duplicate types (identical, near duplicate etc...)

+ High level info,  e.g. - How many duplicates found in total and for each category, is shown in the High Level of Info Tab on the website

<img src="https://nglthu.github.io/gridDocument/img/figure3_highLevel.png" width="100%" heigh="100%">


Fig. 3 Selected High Level of Information with counting


+ Option to filter by 'confidence score' 0 - 1 (score .95 means we are 95% sure the
Files are duplicates): You can filter by the Confidence Score. Here, the filter is provided with different function like equal, is greater than or equal, is less than or equal, and so on.

![alt text](https://nglthu.github.io/gridDocument/img/figure4_OptionOfFilter.png)

Fig. 4 Option of filtering by Confidence Score with different ways of conditions

## Released Code Project File (version v.1.0) and Demo

+ Demo has been captured by opening a file namely demo5.wmv.
+ To run the project, please using the local database (documentDB as attached) for demo and changing connection string in the project.
+ The code has been released by a first version of  gridDocument-1.0.zip

## Wish you enjoy !!
