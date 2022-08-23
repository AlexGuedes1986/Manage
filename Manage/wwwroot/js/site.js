$(document).ready(function () {
    var title = $('#pageHeading h2');
    $('#pageTitle').append(title);
});

//ContactAddEdit page

$(function () {
    $('.btnModalCompany').click(function () {
        var url = $('#modalCompany').data('url');
        $.get(url, function (data) {
            $('.modal-body').html(data);
            $('#modalCompany').modal('show');
        });
        return false;
    });

    function DisplayRecurringDiv() {
        feeStructureDropDowns = [];
        feeStructureDropDowns = $('.ddlFeeStructure');
        feeStructureDropDowns.each(function () {
            if ($(this).val() === 'Recurring') {
                $(this).parent().parent().siblings('.divRecurring').css('display', 'flex');
            }
            else {
                $(this).parent().parent().siblings('.divRecurring').css('display', 'none');
            }
        });
        window.scrollBy(0, 0);
    }

    DisplayRecurringDiv();

    $('#inpProjectName').autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: autocompleteProjectNameUrl,
                data: { 'prefix': request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                },
                error: function (response) {
                    console.log(response.responseText);
                },
                failure: function (response) {
                    console.log(response.responseText);
                }
            });
        },
        select: function (event, ui) {
            $.ajax({
                type: 'post',
                data: { proposalId: ui.item.val },
                url: 'PopulateTasksByProposal',
                success: function (result) {
                    $('#taskByCategories').html(result);
                    DisplayRecurringDiv();
                },
                error: function (errorMessage) {
                    console.log('error populating modal with tasks ' + JSON.stringify(errorMessage));
                }
            });
        }
    });

    $('#btnUpdatePN').click(function () {
        var projNumberEntered = $('.projNumberEditable').val();
        var projectId = $('#btnUpdatePN').data('project-id');
        $.ajax({
            type: 'post',
            url: $(this).data('url'),
            data: { id: $(this).data('project-id'), projNumber: projNumberEntered },
            success: function (response) {
                $('#spnMessage').text('');
                if (JSON.stringify(response).indexOf('error') !== -1) {
                    //error
                    $('#spnMessage').addClass('textRed');
                    $('#spnMessage').text(response.error);
                }
                else {
                    $('#spnMessage').removeClass('textRed');
                    $('#spnMessage').text(response.message);
                    setTimeout(function () {
                        window.location.href = '/Project/ProjectDetails?id=' + projectId;
                    }, 1000);
                }
                console.log(JSON.stringify(response));
            },
            error: function (errMessage) {
                console.log('An error occurred trying to update Project Number: ' + JSON.stringify(errMessage));
            }
        });
        return false;
    });

    let currentProjectStatus = $('#hdnprojectStatus').val();
    $('#projectStatus option[value="' + currentProjectStatus + '"]').attr('selected', 'selected');

    $('#btnUpdateProjStatus').click(function () {
        let projectId = $('#hdnProjectId').val();
        let selectedProjectStatus = $('#projectStatus').children("option:selected").val();
        $.ajax({
            type: 'post',
            url: $(this).data('url'),
            data: { projectId: projectId, projectStatus: selectedProjectStatus },
            success: function (response) {
                $('#spnMessage').text('');
                if (JSON.stringify(response).indexOf('error') !== -1) {
                    //error
                    $.notify('An error occurred trying to update Project Status', 'error');
                    console.log('An error occurred trying to update Project Status : ' + JSON.stringify(response));
                }
                else {
                    $.notify(response.message, 'success');
                }
            },
            error: function (errMessage) {
                $.notify('An error occurred trying to update Project Status', 'error');
                console.log('An error occurred trying to update Project Status: ' + JSON.stringify(errMessage));
            }
        });
        return false;
    });

    $('#btnUpdateContractStatus').click(function () {
        let projectId = $('#hdnProjectId').val();
        let selectedContractStatus = $('#contractStatus').children("option:selected").val();
        $.ajax({
            type: 'post',
            url: $(this).data('url'),
            data: { projectId: projectId, contractStatus: selectedContractStatus },
            success: function (response) {
                $('#spnMessage').text('');
                if (JSON.stringify(response).indexOf('error') !== -1) {
                    //error
                    $.notify('An error occurred trying to update Contract Status', 'error');
                    console.log('An error occurred trying to update Contract Status : ' + JSON.stringify(response));
                }
                else {
                    location.reload();
                }
            },
            error: function (errMessage) {
                $.notify('An error occurred trying to update Contract Status', 'error');
                console.log('An error occurred trying to update Contract Status: ' + JSON.stringify(errMessage));
            }
        });
        return false;
    });

    $('#btnUpdateAllContractUnderSameProjNumber').click(function () {
        $('#pUpdateAllContractsMessage').css('display', 'none');
        let projectId = $('#hdnProjectId').val();
        let selectedContractStatus = $('#contractStatus').children("option:selected").val();
        if (selectedContractStatus.length != 0) {
            $.ajax({
                type: 'post',
                url: $(this).data('url'),
                data: { projectId: projectId, contractStatus: selectedContractStatus },
                success: function (response) {
                    $('#spnMessage').text('');
                    if (JSON.stringify(response).indexOf('error') !== -1) {
                        //error
                        $.notify('An error occurred trying to update Contract Status', 'error');
                        console.log('An error occurred trying to update Contract Status : ' + JSON.stringify(response));
                    }
                    else {
                        location.reload();
                    }
                },
                error: function (errMessage) {
                    $.notify('An error occurred trying to update Contract Status', 'error');
                    console.log('An error occurred trying to update Contract Status: ' + JSON.stringify(errMessage));
                }
            });
        }
        else {
            $('#pUpdateAllContractsMessage').css('display', 'block');
        }
        
        return false;
    });

    $('.checkboxSwitch').on('change', function () {
        if (this.checked) {
            $('.txtAreaSecondParagraph').show();
        }
        else {
            $('.txtAreaSecondParagraph').hide();
        }
    });

    //################# Upload/Replace/Remove Proposal Pdf image

    $('#uploadImagePdf').click(function () {
        var file_data = $('#pdfImage').prop('files')[0];
        if (file_data === undefined || file_data === null) {
            $.notify('Please make sure to select an image to upload', 'error');
            return false;
        }
        var form_data = new FormData();
        form_data.append('file', file_data);
        let urlUploadImage = $('#hdnUploadImageUrl').val();
        let proposalId = $('#proposalId').data('proposal-id');
        form_data.append('proposalId', proposalId);
        form_data.append('operation', $(this).data('operation'));
        $.ajax({
            type: 'post',
            url: urlUploadImage,
            processData: false,
            contentType: false,
            data: form_data,
            success: function (response) {
                if (JSON.stringify(response).indexOf('error') !== -1) {
                    console.log('error in response: ' + response.error)
                    $.notify('An error occurred trying to upload the image', 'error');
                }
                else {
                    $.notify(response.message, 'success');
                }
            },
            error: function (errMessage) {
                $.notify('An error occurred trying to upload the image', 'error');
                console.log('error occurred: ' + JSON.stringify(errMessage));
            }
        });
    });

    $('#btnRemoveImage').click(function () {
        let proposalId = $('#proposalId').data('proposal-id');
        let urlRemove = $(this).data('url-remove-image');
        $.ajax({
            type: 'post',
            url: urlRemove,
            data: { proposalId: proposalId },
            success: function () {
                window.location.href = window.location.href;
            },
            error: function (errMessage) {
                $.notify('An error occurred trying to delete the image', 'error');
                console.log('An error occurred trying to delete the image: ' + JSON.stringify(errMessage));
            }
        });
    });

    $('#btnPartialPayment').click(function () {
        let partialAmount = $('#partialPaymentAmount').val();
        let invoiceId = $('#invoiceId').val();
        let url = $(this).data('url');
        $('.pErrorPayment').css('display', 'none');
        $.ajax({
            type: 'post',
            url: url,
            data: { invoiceId: invoiceId, partialPaymentAmount: partialAmount },
            success: function (message) {
                if (JSON.stringify(message).indexOf('error') !== -1) {
                    $('.pErrorPayment').css('display', 'block');
                    $('.pErrorPayment').text(message.error);
                }
                else {
                    location.reload();
                }

            }
        });
    })

    $('.btnRemovePartialPayment').click(function () {
        let partialPaymentId = $(this).data('partial-payment-id');
        let url = $(this).data('url');
        let invoiceId = $('#invoiceId').val();
        $.ajax({
            type: 'post',
            url: url,
            data: { partialPaymentId: partialPaymentId, invoiceId: invoiceId },
            success: function (response) {
                if (JSON.stringify(response).indexOf('error') !== -1) {
                    $.notify(response.error, 'error');
                }
                else {
                    location.reload();
                }
            }
        })
    });

    $('#btnUpdateInvoice').click(function () {
        let fieldsPossibleUpdate = [];
        let note = $('#txtAreaNote').val();
        let urlEditInvoice = $('#hdnEditInvoice').val();
        let invoiceId = $('#invoiceId').val();
        $('.inpTimesheetRate').each(function () {
            fieldsPossibleUpdate.push({ Value: $(this).val(), InvoiceTimesheetEntryId: $(this).next('.hdnTimesheetEntryId').val(), Type: 'rate' });
        });
        $('.inpTimesheetQty').each(function () {
            fieldsPossibleUpdate.push({ Value: $(this).val(), InvoiceTimesheetEntryId: $(this).next('.hdnTimesheetEntryId').val(), Type: 'qty' });
        });
        $.ajax({
            type: 'post',
            url: urlEditInvoice,
            data: { 'fieldsPossibleUpdate': fieldsPossibleUpdate, 'note': note, 'invoiceId': invoiceId },
            success: function () {
                location.reload();
            },
            error: function (errMessage) {
                $.notify('There was a problem trying to update the Invoice, please contact Administrator', 'error');
                console.log('An error occurred trying to update Invoice: ' + JSON.stringify(errMessage));
            }
        })
    });

});


var urlAutocompleteCompanyName = $('#inpCompanyId').data('url-autocomplete-company');
$('#inpCompanyId').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocompleteCompanyName,
            data: { 'prefix': request.term },
            success: function (data) {
                $('.divCompanyData').empty();
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                $('.divCompanyData').empty();
                console.log(response.responseText);
            },
            failure: function (response) {
                $('.divCompanyData').empty();
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpCompanyId').val(ui.item.label);
        $('.divCompanyData').html('<p class="pnomargin"><span class="bold">Company Name: </span>' + ui.item.label + '</p><p class="pnomargin"><span class="bold">'
            + 'Address: </span>' + ui.item.address1 + ', ' + ui.item.city + ', ' + ui.item.state + ', ' + ui.item.zipcode + '</p>'
            + '<button type="button" id="btnResetCompany" class="btn btn-primary">Reset Company</button>'
            + '<input type="hidden" name="CompanyId" id="contactCompanyId" value=' + ui.item.val + ' />');
        return false;
    }
});

$(document).on('click', '#btnResetCompany', function () {
    $('.divCompanyData').empty();
    $('#inpCompanyId').val('');
    return false;
});

$('#inpCompanyId').attr('autocomplete', 'not');

//Edit Proposal

var updateMessageEditProposal = $('#inpUpdateMessage').val();
if (typeof updateMessageEditProposal !== 'undefined') {
    $.notify(updateMessageEditProposal, 'success');
}
var urlAutocomplete = $('#inpContact').data('url-autocompletecontact');
$('#inpContact').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocomplete,
            data: { 'prefix': request.term },
            success: function (data) {
                $('.divContactData').empty();
                $('.divContactData').show();
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                $('.divContactData').empty();
                $('.divContactData').hide();
                console.log(response.responseText);
            },
            failure: function (response) {
                $('.divContactData').empty();
                $('.divContactData').hide();
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('.divContactData').empty();
        $('.divContactData').show();
        $('#inpContact').val(ui.item.label);
        $('.divContactData').html('<p class="pnomargin"><span class="bold">Contact name: </span> ' + ui.item.label + '</p><p class="pnomargin">'
            + '<span class= "bold"> Address: </span> ' + ui.item.address1 + ', ' + ui.item.city + ', ' + ui.item.state
            + ', ' + ui.item.zipcode + '</p>' + '<p><span class="bold">Company Name: </span>' + ui.item.companyName + '</p>' + '<input type="hidden" name="ContactId"'
            + 'value="' + ui.item.val + '"/><button type="button" id="btnResetClient" class="btn btn-primary">Reset Client</button>');
        $('#contactidLoaded').remove();
        return false;
    }
});

$(document).on('click', '#btnResetClient', function () {
    $('.divContactData').empty();
    $('.divContactData').hide();
    $('#inpContact').val('');
    return false;
});

$('.inputfile').change(function (e) {
    var fileName = '';
    fileName = e.target.value.split('\\').pop();
    if (fileName) {
        $('#spnUploadPic').text(fileName);
    }
    return false;
});


//PopulateTasksByProposal

const urlParamsPopulateTasksByProposal = new URLSearchParams(window.location.search);
$(document).on('click', '.btnAddTaskToProposal', function () {
    var buttonClicked = $(this);
    const proposalId = urlParamsPopulateTasksByProposal.get('proposalId');
    $('input[name="ProposalId"]').val(proposalId);
    let urlAddTask = $('.formTaskFromProposal').data('add-task-url');
    let currentFormData = $(this).closest('.formTaskFromProposal').serialize();
    let currentDivErrorMessage = $(this).parent().parent().find('.divTaskPostMessage');
    $.ajax({
        type: 'post',
        url: urlAddTask,
        data: currentFormData,
        success: function (result) {
            $('.divTaskPostMessage').html('');
            if (JSON.stringify(result).indexOf('error') !== -1) {
                var errors = [];
                $.each(result.errors, function (key, val) {
                    errors.push('<li class="textRed">' + val + '</li>');
                });
                $("<ul/>", {
                    "class": "my-new-list",
                    html: errors.join("")
                }).appendTo(currentDivErrorMessage);
            }
            else {
                var divAddedMessage = buttonClicked.parent().siblings('.totalPrice').find('.divAddedMessage');
                if (divAddedMessage.hasClass('hideElement')) {
                    divAddedMessage.removeClass('hideElement');
                }
                $.notify('The task was added', 'success');

            }
        }
    });
});

$(document).on('click', '#btnImportAllTasks', function () {
    const urlParams = new URLSearchParams(window.location.search);
    const proposalId = urlParams.get('proposalId');
    let urlMultipleTasks = $('#containerTaskExtensions').data('url-multiple-tasks');
    $.ajax({
        type: 'post',
        url: urlMultipleTasks,
        data: { 'tasksExtensionVM': JSON.parse($('#containerTaskExtensions').val()), 'proposalId': proposalId },
        success: function (result) {
            if (JSON.stringify(result).indexOf('error') !== -1) {
                var errors = [];
                $.each(result.errors, function (key, val) {
                    errors.push('<li class="textRed">' + val + '</li>');
                });
                $("<ul/>", {
                    "class": "my-new-list",
                    html: errors.join("")
                });
                console.log('there were errors trying to add the tasks ' + JSON.stringify(errors));
            }
            else {
                $('.divAddedMessage').each(function () {
                    if ($(this).hasClass('hideElement')) {
                        $(this).removeClass('hideElement');
                    }
                });
                $.notify('The tasks were imported', 'success');
            }
        }
    });
});

//Create Proposal

var urlAutocompleteContact = $('#inpContact').data('url-autocompletecontact');
$('#inpContact').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocompleteContact,
            data: { 'prefix': request.term },
            success: function (data) {
                $('.divContactData').empty();
                $('.divContactData').hide();
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                $('.divContactData').empty();
                $('.divContactData').hide();
                console.log(response.responseText);
            },
            failure: function (response) {
                $('.divContactData').empty();
                $('.divContactData').hide();
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpContact').val(ui.item.label);
        $('.divContactData').show();
        let address2 = '';
        if (ui.item.address2 != null && ui.item.address2.length > 0) {
            address2 = '<p class="pnomargin"><span class="bold">Address Line 2:</span> ' + ui.item.address2 + '</p>';
        }
        $('.divContactData').html('<p class="pnomargin"><span class="bold">Contact name: </span> ' + ui.item.label + '</p><p class="pnomargin">'
            + '<span class= "bold"> Address: </span> ' + ui.item.address1 + ', ' + ui.item.city + ', ' + ui.item.state
            + ', ' + ui.item.zipcode + '</p>' + address2 + '<p><span class="bold">Company Name: </span>' + ui.item.companyName + '</p>' + '<input type="hidden" name="ContactId"'
            + 'value="' + ui.item.val + '" /><button type="button" id="btnResetClient" class="btn btn-primary">Reset Client</button>');
        return false;
    }
});

$(document).on('click', '#btnResetClient', function () {
    $('.divContactData').empty();
    $('.divContactData').hide();
    $('#inpContact').val('');
    return false;
});

$(document).ajaxStart(function () {
    $('.imgLoading').show();
});
$(document).ajaxStop(function () {
    $('.imgLoading').hide();
});
var lastItemPosition = 0;
var proposalId = $('#hiddenProposalId').data('proposal-id');
var urlAutocompleteTeamMember = $('#inpTeamMember').data('url-autocompletecontact');
$('#inpTeamMember').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocompleteTeamMember,
            data: { 'prefix': request.term },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        teamMembersAdded = $('.pMargin');
        for (var i = 0; i < teamMembersAdded.length; i++) {
            if ($(teamMembersAdded[i]).text() === ui.item.label) {
                $('#spanTeamMemberValidation').text('Team Member ' + ui.item.label + ' is already on the list');
                return;
            }
        }
        $('#inpTeamMember').val(ui.item.label);
        if ($('.divTeamMember').css('display') === 'none') {
            $('.divTeamMember').css('display', 'inline-block');
        }
        $('#btnResetTeamMember').remove();
        $('.divTeamMember').append('<p class="pMargin">' + ui.item.label + '<i class="fa fa-times faTimesMargin removeTeamMember"></i>'
            + '<input type="hidden" class="teamMemberItem" value="' + ui.item.val + '" /><input type="hidden" class="teamMemberFormattedName" value="'
            + ui.item.label + '" /></p >');

        $('.divTeamMembersContainer').append('<button type="button" id="btnResetTeamMember" class="btn btn-primary">Reset Team Member</button>');
        counterTeamMembers = $('.teamMemberItem').length;
        SetTeamMemberItemsPosition();
        return false;
    }
});

let urlAutocompleteAuthorCreateProposal = $('#inpAuthor').data('url-autocompleteauthor');
$('#inpAuthor').autocomplete({
    source: function (request, response) {
        $('#hidAuthorId').val('');
        $.ajax({
            type: "POST",
            url: urlAutocompleteAuthorCreateProposal,
            data: { 'prefix': request.term },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
        return false;
    },
    select: function (event, ui) {
        $('#inpAuthor').val(ui.item.label);
        $('#hidAuthorId').val(ui.item.val);
        return false;
    }
});

function SetTeamMemberItemsPosition() {
    var itemCounter = 0;
    $('.teamMemberItem').each(function () {
        $(this).attr('name', 'ProposalTeamMember[' + itemCounter + '].ApplicationUserId');
        $(this).siblings('.teamMemberFormattedName').attr('name', 'ProposalTeamMember[' + itemCounter + '].FormattedName');
        itemCounter++;
    });
}

$(document).on('click', '#btnResetTeamMember', function () {
    $('#inpTeamMember').val('');
    $(this).remove();
    return false;
});


$(document).on('click', '.removeTeamMember', function () {
    $(this).parent().remove();
    counterTeamMembers = $('.teamMemberItem').length;
    lastItemPosition = $('.teamMemberItem').last().data('counter');
    SetTeamMemberItemsPosition();
    if (counterTeamMembers === 0) {
        $('.divTeamMember').hide();
    }
    return false;
});

$(function () {
    //Sidebar Active menu
    let currentUrl = window.location.pathname;
    let urlSplitted = currentUrl.split('/');
    $('.sidebar-menu li a[href="/"]').parent().removeClass('active');
    if (urlSplitted[1].length === 0) {
        $('.sidebar-menu li a[href="/"]').parent().addClass('active');
    }
    else {
        $('.sidebar-menu li a[href^="/' + urlSplitted[1] + '"]').parent().addClass('active');
    }

    //View proposal
    $('#btnActivateProject').click(function () {
        $.ajax({
            type: 'post',
            url: $('#btnActivateProject').data('url-activate-project'),
            data: { proposalId: $('#hiddenProposalId').data('proposal-id') },
            success: function (result) {
                if (JSON.stringify(result).indexOf('error') === -1) {
                    $.notify(result.message, 'success');
                    $('#btnEditProposalLink').hide();
                    $('#pActivateError').text('');
                    $('#btnActivateProject').addClass('displayNone');
                    $('.divProjectNumber').removeClass('displayNone');
                }
                else {
                    $('#pActivateError').text(result.error);
                }
            },
            error: function (error) {
                console.log('there was an error trying to activate the project: ' + JSON.stringify(error));
            }
        });
        return false;
    });

    $('.btnSubmitProjectNumber').click(function () {
        let urlSubmitProjectNumber = $(this).data('url-submit-p-number');
        let urlProjects = $(this).data('url-project');
        let projNumberVal = $('#ProjectNumber').val();
        if (projNumberVal.length === 0) {
            $('#pActivateError').text('Please make sure to enter a project number');
            return false;
        }
        $.ajax({
            type: 'post',
            url: urlSubmitProjectNumber,
            data: { proposalId: $('#hiddenProposalId').data('proposal-id'), projectNumber: projNumberVal },
            success: function () {
                $('#pActivateError').text('');
                window.location.href = urlProjects;
            },
            error: function (errorMessage) {
                console.log(JSON.stringify(errorMessage));
            }
        });
        return false;
    });


    //MANAGE TASK ManageTask.cshtml

    if ($('#chbAssignToMyself').attr('checked') === 'checked') {
        $('.divAssignTo').hide();
    }
    else {
        $('.divAssignTo').show();
    }
    let currentStatus = $('#hdnStatus').val();
    $('#selectStatus option[value="' + currentStatus + '"]').attr('selected', 'selected');

    $('#chbAssignToMyself').change(function () {
        if (this.checked) {
            $('.divAssignTo').hide();
        }
        else {
            $('.divAssignTo').show();
        }
    });

    $('#btnAssignTask').click(function () {
        let urlAssignTask = $('#btnAssignTask').data('url-assign-task');
        let typeOfView = $('#hdnTypeOfView').val();
        $.ajax({
            type: 'post',
            url: urlAssignTask,
            data: $('#frmAssignTaskToUser').serialize() + '&createOrUpdate=' + typeOfView,
            success: function (result) {
                $('.pErrorMessageAssignTask').text('');
                if (JSON.stringify(result).indexOf('error') === -1) {
                    $.notify(result.message, 'success');
                    setTimeout(function () { document.location.href = '/Timesheet'; }, 1000);
                }
                else {
                    $('.pErrorMessageAssignTask').text(result.error);
                }
            },
            error: function (errorBody) {
                console.log('an error occurred : ' + JSON.stringify(errorBody));
            }
        });
        return false;
    });

    $('.selectProject').change(function () {
        let urlManageTask = $('#hdnManageTask').data('url-manage-task');
        let urlManageTaskWithParameters = urlManageTask + '?projectId=' + $(this).children('option:selected').val();
        document.location.href = urlManageTaskWithParameters;
        return false;
    });


    //PROJECT DETAILS ProjectDetails.cshtml

    $('.aAddTaskRegular').click(function () {
        let taskExtensionId = $(this).data('taskext-id');
        let urlAddTaskRegular = $(this).data('url-add-task-reg-user');
        $.ajax({
            type: 'post',
            url: urlAddTaskRegular,
            data: { TaskExtensionId: taskExtensionId },
            success: function (result) {
                if (JSON.stringify(result).indexOf('error') === -1) {
                    $.notify(result.message, 'success');
                    $('.pAddToTimesheet').text('');
                    window.location.href = '/timesheet';
                }
                else {
                    $('.pAddToTimesheet').text(result.errorMessage);
                }
            },
            error: function (response) {
                console.log('An error occurred: ' + JSON.stringify(response));
            }
        });
        return false;
    });

    $('.aUnassign').click(function () {
        let elementClicked = $(this);
        let urlUnassign = $(this).data('url-unassign');
        let taskId = $(this).data('task-id');      
        $.ajax({
            type: 'post',
            url: urlUnassign,
            data: { taskExtensionId: taskId },
            success: function (result) {
                $.notify(result.message, 'success');
                elementClicked.parent().siblings('.tdAssignedTo').empty();
                elementClicked.parent().siblings('.editOrManage').find('.aEdit').addClass('displayNone');
                elementClicked.parent().siblings('.editOrManage').find('.aManage').removeClass('displayNone');
                elementClicked.parent().siblings('.tdDueDate').empty();
                elementClicked.parent().siblings('.tdStatus').empty();
                elementClicked.remove();
            },
            error: function (errorMessage) {
                console.log('An error ocurred trying to unassign the task : ' + JSON.stringify(errorMessage));
            }
        });
    });



    //TIMESHEET Timesheet/Index.cshtml

    let currentBillingRole = $('#hdnBillingRole').val();
    $('#inpDateTimesheet').val($('#hdnEffectiveDate').val());
    var taskUserId = '';
    var dateModified = '';
    var $currentElement = '';

    var billingRateHtml = `<div class="row rowts divHourlyRate">
                    <label class="bold col-md-2">Hourly Rate:</label>
                    <div class="col-md-6">
                        <select class="form-control maxWidth130" name="HourlyRate" id="selectHourlyRate" data-url-get-remaining-time="@Url.Action("GetRemainingTime")">
                            <option data-billing-role="Administrative" value="65">$65/Hour</option>
                            <option data-billing-role="GIS" value="110">$110/Hour</option>
                            <option data-billing-role="Field Technician" value="105">$105/Hour</option>
                            <option data-billing-role="Field Biologist" value="130">$130/Hour</option>
                            <option data-billing-role="Wildlife Specialist" value="140">$140/Hour</option>
                            <option data-billing-role="Project Manager" value="150">$150/Hour</option>
                            <option data-billing-role="Director" value="175">$175/Hour</option>
                            <option data-billing-role="Vice President" value="175">$175/Hour</option>
                            <option data-billing-role="President" value="250">$250/Hour</option>
                            <option data-billing-role="Expert Witness" value="350">$350/Hour</option>
                            <option value="375">$375/Hour</option>
                            <option value="4000">$400/Hour</option>
                        </select>
                    </div>
                </div>`;

    $(document).on('click', '.inpHoursEntry', function () {
        $('.spanRemainingTime').text('');
        $('.pErrorMessageTimesheetEntry').text('');
        let requireComment = $(this).parent().parent().data('require-comment');
        var remainingTime = $(this).parent().parent().data('remaining-time');
        var projectDate = new Date($(this).parent().parent().data('project-date'));
        var notToExceed = $(this).parent().parent().data('not-exceed');
        let timesheetEntryType = $(this).siblings('.tsEntryType').val();    
        $('#notToExceedContainer').data('not-to-exceed-val', notToExceed);
        let feeStructure = $(this).parent().parent().data('fee-structure');
        $('#hdnRequireComment').val(requireComment);
        $currentElement = $(this);
        $('.spanSelectedTask').text($(this).parent().siblings('.tdTaskTitle').text());
        taskUserId = $(this).parent().siblings('.tdTaskTitle').data('taskuserid');
        dateModified = $(this).data('selected-date');
        $('.spanRemainingTime').remove();
        $('.spanSelectedDate').text($(this).data('selected-date'));
        $('#divHourlyRateContainer').html(billingRateHtml);
        if ($(this).val() > 0) {
            $('.inpHoursWorked').val($(this).val());
            $('#formTimesheetEntryId').val($(this).siblings('.tsEntryId').val());
            $(this).siblings($('#txtaComment').text($(this).siblings('.tsEntryNote').val()));
            if (timesheetEntryType === 'tracking') {
                $('#rdTracking').prop('checked', true);
            }
            else {
                $('#rdBilled').prop('checked', true);
            }
        }
        else {

            $('.inpHoursWorked').val('');
            $('#formTimesheetEntryId').val('');
            $('#txtaComment').text('');
            if (feeStructure.toLowerCase() === 'flat fee' || feeStructure.toLowerCase() === 'recurring'
                || feeStructure.toLowerCase() === 'per event') {
                $('#rdBilled').prop('checked', false);
                $('#rdTracking').prop('checked', true);
            }
            else {
                $('#rdTracking').prop('checked', false);
                $('#rdBilled').prop('checked', true);
            }
        }
        if (requireComment === 'True') {
            $('.pRequireComment').show();
        }
        if (notToExceed === 'True') {
            if (remainingTime >= 0) {
                $('.divHoursWorked').append('<span class="spanRemainingTime">You can add no more than ' + remainingTime + ' hours</span>');
            }
            else {
                $('.divHoursWorked').append('<span class="spanRemainingTime textRed">You have exceeded the limit of hours for this task, please notify related Project Manager</span>');
            }
            $('.divHoursWorked').append('<input type="hidden" name="RemainingTimeHourlyNotToExceed" value="' + remainingTime + '">');
        }
        else {
            $('.pRequireComment').hide();
        }       
        if (feeStructure.toLowerCase() === 'flat fee' || feeStructure.toLowerCase() === 'recurring'
            || feeStructure.toLowerCase() === 'per event') {
            $('#selectHourlyRate').removeAttr('name');
            $('.divHourlyRate').hide();
        }
        else {
            $('#selectHourlyRate').attr('name', 'HourlyRate');
            $('.divHourlyRate').show();
        }
        $('#selectHourlyRate').val($('*[data-billing-role="' + currentBillingRole + '"]').val());
        $('#formTimesheetEntry').show();
        return false;
    });

    $(document).on('change', '#selectHourlyRate', function () {
        let notToExceedVal = $('#notToExceedContainer').data('not-to-exceed-val');
        if (notToExceedVal === 'True') {
            var hoursEntered = $('.inpHoursWorked').val();
            var hourlyRateSelected = $('#selectHourlyRate').val();
            $.ajax({
                type: 'get',
                url: $('#CalculateHourlyNotToExceedRemTime').data('url'),
                data: { taskUserId: taskUserId, hoursEntered: hoursEntered, hourlyRateSelected: hourlyRateSelected },
                success: function (response) {                 
                    if (response.remainingTime >= 0) {
                        $('.spanRemainingTime').removeClass('textRed');
                        $('.spanRemainingTime').text('You can add no more than ' + response.remainingTime + ' hours');
                    }
                    else {
                        $('.spanRemainingTime').addClass('textRed');
                        $('.spanRemainingTime').text('You have exceeded the limit of hours for this task, please notify related Project Manager');
                    }
                },
                error: function (errMessage) {
                    console.log('An error occurred trying to get remaining Hourly Not To Exceed Calculation: ' + JSON.stringify(errMessage));
                }
            });
        }
        return false;
    });

    $(document).on('change', '.inpHoursWorked', function () {
        let notToExceedVal = $('#notToExceedContainer').data('not-to-exceed-val');
        $currentElement.val($(this).val());
        if (notToExceedVal === 'True') {
            var hoursEntered = $(this).val();
            var hourlyRateSelected = $('#selectHourlyRate').val();
            $.ajax({
                type: 'get',
                url: $('#CalculateHourlyNotToExceedRemTime').data('url'),
                data: { taskUserId: taskUserId, hoursEntered: hoursEntered, hourlyRateSelected: hourlyRateSelected },
                success: function (response) {                 
                    if (response.remainingTime >= 0) {
                        $('.spanRemainingTime').removeClass('textRed');
                        $('.spanRemainingTime').text('You can add no more than ' + response.remainingTime + ' hours');
                    }
                    else {
                        $('.spanRemainingTime').addClass('textRed');
                        $('.spanRemainingTime').text('You have exceeded the limit of hours for this task, please notify related Project Manager');
                    }
                },
                error: function (errMessage) {
                    console.log('An error occurred trying to get remaining Hourly Not To Exceed Calculation: ' + JSON.stringify(errMessage));
                }
            });
        }
        return false;
    });

    $('#btnApplyTimesheet').click(function (e) {
        $.ajax({
            type: 'post',
            url: $('#btnApplyTimesheet').data('url-add-ts-entry'),
            data: $('#formTimesheetEntry').serialize() + '&TaskUserId=' + taskUserId + '&DateModified=' + dateModified,
            success: function (result) {         
                if (JSON.stringify(result).indexOf('error') === -1) {
                    if (result.entryType == 'billed') {
                        $('#iRefreshTimesheets').css('display', 'inline-block');                        
                    }
                    console.log('result is ' +  JSON.stringify(result));
                    if (result.isComplete) {
                        $currentElement.parent().siblings().find('.inpHoursEntry').attr('disabled', 'disabled');
                        $currentElement.attr('disabled', 'disabled');
                        $currentElement.parent().siblings('.tdTaskStatus').text('Complete');
                    }             
                    if ($currentElement.siblings('.tsEntryType').length) {
                        $currentElement.siblings('.tsEntryType').val(result.entryType);
                    }
                    else {
                        $currentElement.append('<input type="hidden" class="tsEntryType" value='+ result.entryType +'>')
                    }
                    if ($currentElement.siblings('.tsEntryId').length) {
                        $currentElement.siblings('.tsEntryId').val(result.id);
                    }
                    else {
                        $currentElement.after('<input type="hidden" class="tsEntryId" value="' + result.id + '" />');
                    }
                    if ($currentElement.siblings('.tsEntryNote').length) {
                        $currentElement.siblings('.tsEntryNote').val(result.note);
                    }
                    else {
                        let note = '';
                        if (result.note != null) {
                            note = result.note;
                        }
                        $currentElement.after('<input type="hidden" class="tsEntryNote" value="' + note + '" />');
                    }
                    $.notify('Timesheet entry has been updated', 'success');
                    $('.pErrorMessageTimesheetEntry').text('');
                    $('#formTimesheetEntry').hide();
                }
                else {
                    $('.pErrorMessageTimesheetEntry').text(result.error);
                }
            },
            error: function (errorMessage) {
                console.log('An error took place trying to create Timesheet entry : ' + JSON.stringify(errorMessage));
            }
        });
        return false;
    });

    $(document).on('click', '#iRefreshTimesheets', function () {
        location.reload();
    });

    $('#selectUserTS').change(function () {
        let selectedUserId = $(this).val()
        $('#hdnUserIdTS').val(selectedUserId);
        let urlIndexWithParameters = CreateTimesheetUrlWithParameters();
        document.location.href = urlIndexWithParameters;
        return false;
    });

    $('#selectUserTS > option').each(function () {
        if ($(this).data('selected-option') === 'True') {
            $(this).prop('selected', true);
        }

        if ($(this).data('approved') === 'True') {
            $(this).addClass('colorRed');
        }
    });

    $('.btnApprove').click(function () {
        let currentDate = $('#spnEffectiveDate').data('effective-date');
        let timesheetApprovedDateRange = new Object();
        timesheetApprovedDateRange.DateStart = $('#hdnDateStart').val();
        timesheetApprovedDateRange.DateEnd = $('#hdnDateEnd').val();
        timesheetApprovedDateRange.UserId = $('#hdnUserId').val();
        let urlApprove = $('#hdnApproveUrl').val();
        $.ajax({
            type: 'post',
            url: urlApprove,
            data: { timesheetApprovedDateRange: timesheetApprovedDateRange },
            success: function (result) {
                let urlIndexWithParameters = CreateTimesheetUrlWithParameters();
                document.location.href = urlIndexWithParameters;
            },
            error: function (errorMessage) {
                console.log('An error occurred trying to approve timesheets ' + JSON.stringify(errorMessage));
            }
        });
        return false;
    });

    $('.btnUnapprove').click(function () {
        let approvedId = $('#hdnApprovedTSId').val();
        let urlUnapprove = $('#urlUnapprove').val();
        $.ajax({
            type: 'post',
            url: urlUnapprove,
            data: { approvedTimesheetId: approvedId },
            success: function () {
                let urlIndexWithParameters = CreateTimesheetUrlWithParameters();
                document.location.href = urlIndexWithParameters;
            },
            error: function (errorMessage) {
                console.log('An error occurred trying to unnapprove timesheet ' + JSON.stringify(errorMessage));
            }
        });
        return false;
    });

    function CreateTimesheetUrlWithParameters() {
        let urlIndex = $('#hdnUserIdTS').data('url-index');
        let effectiveDate = $('#spnEffectiveDate').data('effective-date');
        let encodedEffectiveDate = encodeURIComponent(effectiveDate);
        let userId = $('#hdnUserIdTS').val();
        let urlIndexWithParameters = urlIndex + '?date=' + encodedEffectiveDate + '&userId=' + userId;
        return urlIndexWithParameters;
    }

    $('.inpHoursEntry').on('input', function () {
        $('.inpHoursWorked').val($(this).val());
        $('.inpHoursWorked').trigger('change');
        return false;
    });

    $('#btnResetTS').click(function () {
        document.location.href = $('#hdnUserIdTS').data('url-index');
        return false;
    });

    $('.aRemoveFromTimesheet').click(function () {
        let taskUserIdToHide = $(this).parent().siblings('.tdTaskTitle').data('taskuserid');
        let urlRemoveFromTimesheet = $(this).data('remove-from-timesheet');
        $.ajax({
            type: 'post',
            url: urlRemoveFromTimesheet,
            data: { taskUserId: taskUserIdToHide },
            success: function (result) {
                let urlIndexWithParameters = CreateTimesheetUrlWithParameters();
                document.location.href = urlIndexWithParameters;
            },
            error: function (errorMessage) {
                console.log('An error occurred: ' + JSON.stringify(errorMessage));
            }
        });
        return false;
    });

});

$(document).on('change', '.ddlFeeStructure', function () {
    $(this).parentsUntil('form').find('#InstancePrice').val(0);
    $(this).parentsUntil('form').find('#NumberOfInstances').val(0);
    $(this).parentsUntil('form').find('#TotalPrice').val(0);
    $(this).parentsUntil('form').find('#NotToExceedTotalPrice').val(0);

    var valueSelected = $(this).val();
    $(this).parentsUntil('form').find('.divEventPrice').show();
    $(this).parentsUntil('form').find('.divTotalPrice').show();
    $(this).parentsUntil('form').find('.divNotToExceedTotalPrice').hide();
    $(this).parentsUntil('form').find('.divRecurring').hide();
    $(this).parentsUntil('form').find('#TotalPrice').attr('readonly', false);
    feeStructureSelected = valueSelected;
    if (valueSelected === 'Flat Fee') {
        $(this).parentsUntil('form').find('.divEventPrice').hide();
    }
    if (valueSelected === 'Hourly') {
        $(this).parentsUntil('form').find('.divEventPrice').hide();
        $(this).parentsUntil('form').find('.divTotalPrice').hide();
    }
    if (valueSelected === 'Hourly Not To Exceed') {
        $(this).parentsUntil('form').find('.divEventPrice').hide();
        $(this).parentsUntil('form').find('.divTotalPrice').hide();
        $(this).parentsUntil('form').find('.divNotToExceedTotalPrice').show();
    }
    if (valueSelected === 'Per Event') {
        $(this).parentsUntil('form').find('.divTotalPrice').hide();
    }
    if (valueSelected === 'Recurring') {
        $(this).parentsUntil('form').find('.divRecurring').show();
        $(this).parentsUntil('form').find('input[name="TotalPrice"]').attr('readonly', 'readonly');
    }
    return false;
});

$('#modalAddEditTask').on('shown.bs.modal', function () {
    let valueSelected = $('.ddlFeeStructure').val();
    FeeStructure(valueSelected, true);
});

$(function () {
    SetupTasks();
});

function SetupTasks() {
    $('.ddlFeeStructure').each(function () {
        var valueSelected = $(this).val();
        $(this).parentsUntil('form').find('.divEventPrice').show();
        $(this).parentsUntil('form').find('.divTotalPrice').show();
        $(this).parentsUntil('form').find('.divNotToExceedTotalPrice').hide();
        $(this).parentsUntil('form').find('.divRecurring').hide();
        $(this).parentsUntil('form').find('#TotalPrice').attr('readonly', false);
        feeStructureSelected = valueSelected;
        if (valueSelected === 'Flat Fee') {
            $(this).parentsUntil('form').find('.divEventPrice').hide();
        }
        if (valueSelected === 'Hourly') {
            $(this).parentsUntil('form').find('.divEventPrice').hide();
            $(this).parentsUntil('form').find('.divTotalPrice').hide();
        }
        if (valueSelected === 'Hourly Not To Exceed') {
            $(this).parentsUntil('form').find('.divEventPrice').hide();
            $(this).parentsUntil('form').find('.divTotalPrice').hide();
            $(this).parentsUntil('form').find('.divNotToExceedTotalPrice').show();
        }
        if (valueSelected === 'Per Event') {
            $(this).parentsUntil('form').find('.divTotalPrice').hide();
        }
        if (valueSelected === 'Recurring') {
            $(this).parentsUntil('form').find('.divRecurring').show();
            $(this).parentsUntil('form').find('input[name="TotalPrice"]').attr('readonly', 'readonly');
        }
    });
}

$(document).on('click', '.btnCloseModal', function () {
    $('#modalAddEditTask').modal('hide');
    return false;
});

var feeStructureSelected = '';

function FeeStructure(valueSelected, shouldPrepopulate) {
    if (!shouldPrepopulate) {
        $('#InstancePrice').val(0);
        $('#NumberOfInstances').val(0);
        $('#TotalPrice').val(0);
        $('.divNotToExceedTotalPrice').val(0);
    }
    $('.divEventPrice').hide();
    $('.divTotalPrice').hide();
    $('.divNotToExceedTotalPrice').hide();
    $('.divRecurring').hide();
    $('#TotalPrice').attr('readonly', false);
    feeStructureSelected = valueSelected;
    if (valueSelected === 'Flat Fee') {
        $('.divEventPrice').hide();
        $('.divTotalPrice').show();
    }
    if (valueSelected === 'Hourly') {
        $('.divEventPrice').hide();
        $('.divTotalPrice').hide();
    }
    if (valueSelected === 'Hourly Not To Exceed') {
        $('.divEventPrice').hide();
        $('.divTotalPrice').hide();
        $('.divNotToExceedTotalPrice').show();
    }
    if (valueSelected === 'Per Event') {
        $('.divTotalPrice').hide();
        $('.divEventPrice').show();
    }
    if (valueSelected === 'Recurring') {
        $('.divRecurring').show();
        $('.divEventPrice').show();
        $('.divNote').show();
        $('.divTotalPrice').show();
        $('input[name="TotalPrice"]').attr('readonly', 'readonly');
    }
    return false;
}

$(document).on('change', '#InstancePrice', function () {
    let instancePrice = $(this).val();
    let numberOfInstances = $(this).parents('.parentTaskDiv').find('input[name="NumberOfInstances"]').val();
    let closerTotalPriceElement = $(this).parents('.parentTaskDiv').find('input[name="TotalPrice"]');
    CalculateTotalRecurringPrice(instancePrice, numberOfInstances, closerTotalPriceElement);
    return false;
});

$(document).on('change', '#NumberOfInstances', function () {
    let numberOfInstances = $(this).val();
    let instancePrice = $(this).parents('.parentTaskDiv').find('input[name="InstancePrice"]').val();
    let closerTotalPriceElement = $(this).parents('.parentTaskDiv').find('input[name="TotalPrice"]');
    CalculateTotalRecurringPrice(instancePrice, numberOfInstances, closerTotalPriceElement);
    return false;
});

function CalculateTotalRecurringPrice(eventPrice, numberOfInstances, closerTotalPriceElement) {
    if (feeStructureSelected === 'Recurring') {
        $(closerTotalPriceElement).val(eventPrice * numberOfInstances);
    }
}

//Adjust textarea to content in Create/Edit Proposal pages
function textAreaAdjust(o) {
    o.style.height = "1px";
    o.style.height = 25 + o.scrollHeight + "px";
}


//Proposals TasksByCategory page
var radioButtonValue = 'category';
$(function () {
    $("input[name='radioSearch']").click(function () {
        radioButtonValue = $(this).val();
        $('#inpTask').autocomplete('close').val('');
        $('#taskByCategories').empty();
        $('#inpTask').attr('placeholder', 'Please type ' + $(this).parent().text());
    });
});

var autocompleteTaskUrl = $('#inpTask').data('url-autocompletetask');
var autocompleteProjectNameUrl = $('#inpProjectName').data('url-autocompleteprojectname');
$('#inpTask').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: autocompleteTaskUrl,
            data: { 'prefix': request.term, 'searchType': radioButtonValue },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        if (radioButtonValue === 'category') {
            $.ajax({
                type: 'post',
                data: { category: ui.item.label, proposalId: $('#proposalId').data('proposal-id') },
                url: 'PopulateTaskByCategory',
                success: function (result) {
                    $('#taskByCategories').html(result);
                },
                error: function (errMessage) {
                    console.log('error populating tasks by Category Search ' + JSON.stringify(errMessage));
                }
            });
        }

        if (radioButtonValue === 'taskNumber') {
            $.ajax({
                type: 'post',
                data: { taskId: ui.item.id },
                url: 'OpenTask',
                success: function (result) {
                    $('.modal-body-task').html(result);
                    $('#formAddTaskModal').append('<input type="hidden" name="ProposalId" value="' + $('#proposalId').data('proposal-id') + '" />');
                    $('#modalAddEditTask').modal('show');
                },
                error: function () {
                    console.log('error populating tasks by Task Number ' + JSON.stringify(errMessage));
                }
            });
        }

        if (radioButtonValue === 'taskTitle') {
            $.ajax({
                type: 'post',
                data: { taskId: ui.item.id },
                url: 'OpenTask',
                success: function (result) {
                    $('.modal-body-task').html(result);
                    $('#formAddTaskModal').append('<input type="hidden" name="ProposalId" value="' + $('#proposalId').data('proposal-id') + '" />');
                    $('#modalAddEditTask').modal('show');
                },
                error: function () {
                    console.log('error populating tasks by Task Title ' + JSON.stringify(errMessage));
                }
            });
        }

    }
});

$(document).on('click', '.addTask', function () {
    let url = $('#inpTaskUrl').data('task-url');
    var taskId = $(this).siblings('#taskVM_Id').val();
    $.ajax({
        type: 'get',
        data: { taskId: taskId },
        url: url,
        success: function (result) {
            $('#modalAddEditTask').find('.modal-title').text('Add task to Proposal');
            $('.modal-body-task').html(result);
            $('#formAddTaskModal').append('<input type="hidden" name="ProposalId" value="' + $('#proposalId').data('proposal-id') + '" />');
            $('#modalAddEditTask').modal('show');
        },
        error: function () {
            console.log('error populating modal with tasks');
        }
    });
});

$(document).on('click', '#btnAddTaskProp', function () {
    let urlPostTask = $('#inpPostTaskUrl').data('post-task');
    $.ajax({
        type: 'post',
        url: urlPostTask,
        data: $('#formAddTaskModal').serialize(),
        success: function (result) {
            $('#divTaskPostMessage').html('');
            if (JSON.stringify(result).indexOf('error') !== -1) {
                var errors = [];
                $.each(result.errors, function (key, val) {
                    errors.push('<li class="textRed">' + val + '</li>');
                });
                $("<ul/>", {
                    "class": "my-new-list",
                    html: errors.join("")
                }).appendTo("#divTaskPostMessage");
            }
            else {
                $('.lblTaskVM').each(function () {
                    if ($(this).text() === result.taskName) {
                        $(this).parent().append(`<i class="fa fa-edit marginLeft10 editTask cursorPointer" title="Edit"></i>
                <i class="fa fa-trash marginLeft10 removeTask cursorPointer" title="Remove"></i>`);
                        $(this).siblings('.addTask').hide();
                        $(this).siblings('.taskExtensionId').val(result.taskId);
                    }
                });
                $('.divTasksAdded').append('<p><i class="ion-checkmark"> </i>' + result.taskName + '</p>');
                $.notify('The task was added', 'success');
                $('#modalAddEditTask').modal('hide');
            }
        },
        error: function (result) {
            console.log('error posting task, message: ' + JSON.stringify(result));
        }
    });
});

$(document).on('click', '.editTask', function () {
    let urlEditTask = $('#inpEditTask').data('edit-task-url');
    let taskToEditId = $(this).siblings('.taskExtensionId').val();
    let proposalIdEditTask = $('#proposalId').data('proposal-id');
    $('#formAddTaskModal').hide();
    $.ajax({
        type: 'get',
        data: { taskId: taskToEditId, proposalId: proposalIdEditTask },
        url: urlEditTask,
        success: function (result) {
            $('#modalAddEditTask').find('.modal-title').text('Edit Task');
            $('.modal-body-task').html(result);
            $('#modalAddEditTask').append('<input type="hidden" name="ProposalId" value="' + proposalIdEditTask + '" />');
            $('#modalAddEditTask').modal('show');
        }
    });
});

$(document).on('click', '#btnEditTaskProp', function () {
    let urlEditTask = $('#inpEditTask').data('edit-task-url');
    $.ajax({
        type: 'post',
        url: urlEditTask,
        data: $('#formEditTaskModal').serialize(),
        success: function (result) {
            $('#divTaskEditMessage').html('');
            if (JSON.stringify(result).indexOf('error') !== -1) {
                var errors = [];
                $.each(result.errors, function (key, val) {
                    errors.push('<li class="textRed">' + val + '</li>');
                });
                $("<ul/>", {
                    "class": "my-new-list",
                    html: errors.join("")
                }).appendTo("#divTaskEditMessage");
            }
            else {
                $('.lblTaskVM').each(function () {
                    if ($(this).text() === result.taskName) {
                        $(this).append(`<i class="fa fa-edit marginLeft10 editTask cursorPointer" title="Edit"></i>
                                <i class="fa fa-trash marginLeft10 removeTask cursorPointer" title="Remove"></i>`);
                    }
                });
                $.notify('The task was updated', 'success');
                $('#modalAddEditTask').modal('hide');
            }
        },
        error: function (result) {
            console.log('error posting task, message: ' + JSON.stringify(result));
        }
    });
});

$(document).on('click', '.removeTask', function () {
    var currentElement = $(this);
    let taskIdToRemove = $(this).siblings('.taskExtensionId').val();
    let proposalIdRemoveTask = $('#proposalId').data('proposal-id');
    $.ajax({
        type: 'post',
        url: $('#inpRemoveTask').data('url-remove'),
        data: { taskId: taskIdToRemove, proposalId: proposalIdRemoveTask },
        success: function (result) {
            currentElement.parent().append('<i class="fa fa-plus cursorPointer addTask marginLeft10" title="Add"></i>');
            currentElement.siblings('.editTask').remove();
            currentElement.remove();
            $.notify(result.success, 'success');
        }, error: function (errMessage) {
            console.log('An error occurred trying to remove task from Proposal: ' + JSON.stringify(errMessage));
        }
    });
});

$(document).on('change', '.checkboxInvoicePaid', function () {
    let status = '';
    let invoiceId = parseInt($(this).val());
    if ($(this).is(':checked')) {
        status = 'Paid in Full';
    }
    else {
        status = 'Pending';
    }

    $.ajax({
        type: 'post',
        url: $('#hdnUpdateInvoiceStatus').val(),
        data: { invoiceId: invoiceId, status: status },
        success: function () {
            $('#invoiceTable').data('kendoGrid').dataSource.read();
        }, error: function (errMessage) {
            $.notify('An error ocurred trying to update Invoice Status');
            console.log('error trying to update Invoice Status: ' + JSON.stringify(errMessage));
        }

    });
})

$(document).on('change', '.checkboxFinalized', function () {
    let finalized = false;
    let status = '';
    let invoiceId = parseInt($(this).val());
    if ($(this).is(':checked')) {
        finalized = true;
        status = 'Finalized';
    }
    else {
        finalized = false;
        status = 'Pending';
    }

    $.ajax({
        type: 'post',
        url: $('#hdnUpdateInvoiceFinalized').val(),
        data: { invoiceId: invoiceId, finalized: finalized, status: status },
        success: function () {
            $('#invoiceTable').data('kendoGrid').dataSource.read();
        }, error: function (errMessage) {
            $.notify('An error ocurred trying to change Invoice Finalized Status');
            console.log('error trying to update Invoice Finalized: ' + JSON.stringify(errMessage));
        }

    });
})

//Autocomplete Invoice Generation
var urlAutocompleteInvoice = $('#inpInvoiceGeneration').data('url-autocomplete');
$('#inpInvoiceGeneration').autocomplete({
    source: function (request, response) {
        let typeSearch = $('.radioInvoiceSearch:checked').val();
        $.ajax({
            type: "POST",
            url: urlAutocompleteInvoice,
            data: { 'prefix': request.term, 'typeSearch': typeSearch },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpInvoiceGeneration').val(ui.item.label);
        return false;
    }
});

//Autocomplete Report Generation
var urlAutocompleteReport = $('#inpReportGeneration').data('url-autocomplete');
$('#inpReportGeneration').autocomplete({
    source: function (request, response) {
        let typeSearch = $('.radioReportSearch:checked').val();
        $.ajax({
            type: "POST",
            url: urlAutocompleteReport,
            data: { 'prefix': request.term, 'typeSearch': typeSearch },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpReportGeneration').val(ui.item.label);
        return false;
    }
});

var urlAutocompleteBalanceReport = $('#inpOpenBalanceReport').data('url-autocomplete');
$('#inpOpenBalanceReport').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocompleteBalanceReport,
            data: { 'prefix': request.term },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpOpenBalanceReport').val(ui.item.label);
        $('#inppnCustomerOpenBalance').val(ui.item.val);
        return false;
    }
});

var urlCreditMemoReport = $('#inpCreditMemoReport').data('url-autocomplete');
$('#inpCreditMemoReport').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlCreditMemoReport,
            data: { 'prefix': request.term },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        $('#inpCreditMemoReport').val(ui.item.label);
        $('#hdnCreditMemo').val(ui.item.val);
        return false;
    }
});

$(document).on('input', '.inpHoursEntry', function () {
    $('.inpHoursWorked').val($(this).val());
    $('.inpHoursWorked').trigger('change');
    return false;
})

var urlAutocompleteUserTaskAssignment = $('#inpUserTaskAssign').data('url-autocomplete-user');
$('#inpUserTaskAssign').autocomplete({
    source: function (request, response) {
        $.ajax({
            type: "POST",
            url: urlAutocompleteUserTaskAssignment,
            data: { 'prefix': request.term },
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }));
            },
            error: function (response) {
                console.log(response.responseText);
            },
            failure: function (response) {
                console.log(response.responseText);
            }
        });
    },
    select: function (event, ui) {
        userAdded = $('.pMargin');
        let userAlreadyAssigned = false;
        for (var i = 0; i < userAdded.length; i++) {
            if ($(userAdded[i]).text() === ui.item.label) {
                userAlreadyAssigned = true;
                return;
            }
        }
        if (userAlreadyAssigned) {
            $('#spanUserTaskValidation').text('User ' + ui.item.label + ' is already on the list');
        }
        else {
            $('#spanUserTaskValidation').text('');
        }
        $('#inpUserTaskAssign').val(ui.item.label);
        if ($('.divUserTaskAssignment').css('display') === 'none') {
            $('.divUserTaskAssignment').css('display', 'inline-block');
        }
        $('#btnResetUserTaskAssignment').remove();
        $('.divUserTaskAssignment').append('<p class="pMargin">' + ui.item.label + '<i class="fa fa-times faTimesMargin removeUserAssigned"></i>'
            + '<input type="hidden" class="userAssignedItem" value="' + ui.item.val + '" /><input type="hidden" class="userAssignedFormattedName" value="'
            + ui.item.label + '" /></p >');

        $('#btnClearInputUserAssigned').removeClass('displayNone');
        counterUsersAssigned = $('.userAssignedItem').length;
        SetUserAssignedItemsPosition();
        return false;
    }
});

function SetUserAssignedItemsPosition() {
    var itemCounter = 0;
    $('.userAssignedItem').each(function () {
        $(this).attr('name', 'TaskUsersAssigned[' + itemCounter + '].ApplicationUserId');
        $(this).siblings('.userAssignedFormattedName').attr('name', 'TaskUsersAssigned[' + itemCounter + '].FormattedName');
        itemCounter++;
    });
}

$(document).on('click', '#btnClearInputUserAssigned', function () {
    $('#inpUserTaskAssign').val('');
    $('#btnClearInputUserAssigned').addClass('displayNone');
    return false;
});


$(document).on('click', '.removeUserAssigned', function () {
    $(this).parent().remove();
    counterUsersAssigned = $('.userAssignedItem').length;
    lastItemPosition = $('.userAssignedItem').last().data('counter');
    SetUserAssignedItemsPosition();
    if (counterUsersAssigned === 0) {
        $('.divUserTaskAssignment').hide();
    }
    return false;
});