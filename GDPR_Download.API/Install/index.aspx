<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="GDPR_Download.API.Views.Install.index" %>

<!DOCTYPE html>
<style>
    .wizard {
        margin: 20px auto;
        background: #fff;
    }

        .wizard .nav-tabs {
            position: relative;
            margin: 40px auto;
            margin-bottom: 0;
            border-bottom-color: #e0e0e0;
        }

        .wizard > div.wizard-inner {
            position: relative;
        }

    .connecting-line {
        height: 2px;
        background: #e0e0e0;
        position: absolute;
        width: 80%;
        margin: 0 auto;
        left: 0;
        right: 0;
        top: 50%;
        z-index: 1;
    }

    .wizard .nav-tabs > li.active > a, .wizard .nav-tabs > li.active > a:hover, .wizard .nav-tabs > li.active > a:focus {
        color: #555555;
        cursor: default;
        border: 0;
        border-bottom-color: transparent;
    }

    span.round-tab {
        width: 70px;
        height: 70px;
        line-height: 70px;
        display: inline-block;
        border-radius: 100px;
        background: #fff;
        border: 2px solid #e0e0e0;
        z-index: 2;
        position: absolute;
        left: 0;
        text-align: center;
        font-size: 25px;
    }

        span.round-tab i {
            color: #555555;
        }

    .wizard li.active span.round-tab {
        background: #fff;
        border: 2px solid #5bc0de;
    }

        .wizard li.active span.round-tab i {
            color: #5bc0de;
        }

    span.round-tab:hover {
        color: #333;
        border: 2px solid #333;
    }

    .wizard .nav-tabs > li {
        width: 25%;
    }

    .wizard li:after {
        content: " ";
        position: absolute;
        left: 46%;
        opacity: 0;
        margin: 0 auto;
        bottom: 0px;
        border: 5px solid transparent;
        border-bottom-color: #5bc0de;
        transition: 0.1s ease-in-out;
    }

    .wizard li.active:after {
        content: " ";
        position: absolute;
        left: 46%;
        opacity: 1;
        margin: 0 auto;
        bottom: 0px;
        border: 10px solid transparent;
        border-bottom-color: #5bc0de;
    }

    .wizard .nav-tabs > li a {
        width: 70px;
        height: 70px;
        margin: 20px auto;
        border-radius: 100%;
        padding: 0;
    }

        .wizard .nav-tabs > li a:hover {
            background: transparent;
        }

    .wizard .tab-pane {
        position: relative;
        padding-top: 50px;
    }

    .wizard h3 {
        margin-top: 0;
    }

    @media( max-width : 585px ) {

        .wizard {
            width: 90%;
            height: auto !important;
        }

        span.round-tab {
            font-size: 16px;
            width: 50px;
            height: 50px;
            line-height: 50px;
        }

        .wizard .nav-tabs > li a {
            width: 50px;
            height: 50px;
            line-height: 50px;
        }

        .wizard li.active:after {
            content: " ";
            position: absolute;
            left: 35%;
        }
    }
</style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.3.1.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <section>
                    <div class="wizard">
                        <div class="wizard-inner">
                            <div class="connecting-line"></div>
                            <ul class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="active">
                                    <a href="#step1" data-toggle="tab" aria-controls="step1" role="tab" title="SMTP">
                                        <span class="round-tab">
                                            <i class="glyphicon glyphicon-hdd"></i>
                                        </span>
                                    </a>
                                </li>
                                <li role="presentation" class="disabled">
                                    <a href="#step2" data-toggle="tab" aria-controls="step2" role="tab" title="SQL">
                                        <span class="round-tab">
                                            <i class="glyphicon glyphicon-cloud"></i>
                                        </span>
                                    </a>
                                </li>
                                <li role="presentation" class="disabled">
                                    <a href="#step3" data-toggle="tab" aria-controls="step3" role="tab" title="Övrigt">
                                        <span class="round-tab">
                                            <i class="glyphicon glyphicon-edit"></i>
                                        </span>
                                    </a>
                                </li>
                                <li role="presentation" class="disabled">
                                    <a href="#complete" data-toggle="tab" aria-controls="complete" role="tab" title="Klar att köra">
                                        <span class="round-tab">
                                            <i class="glyphicon glyphicon-ok"></i>
                                        </span>
                                    </a>
                                </li>
                            </ul>
                        </div>

                        <form role="form">
                            <div class="tab-content">
                                <div class="tab-pane active" role="tabpanel" id="step1">
                                    <div class="col-md-6 col-md-offset-3">
                                        <h3>SMTP Inställningar:</h3>
                                        <p style="font-size: 14px; margin-bottom: 2px;">Server:</p>
                                        <asp:TextBox id="smtp_Server" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Port:</p>
                                        <asp:TextBox id="smtp_Port"  class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Användarnamn:</p>
                                        <asp:TextBox id="smtp_Username"  class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">lösenord:</p>
                                        <asp:TextBox id="smtp_Password"  class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <button type="button" class="btn form-control btn-primary next-step">Nästa</button>
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="step2">
                                    <div class="col-md-6 col-md-offset-3">
                                        <h3>SQL Inställningar:</h3>
                                        <p style="font-size: 14px; margin-bottom: 2px;">Server:</p>
                                        <asp:TextBox id="sql_Server" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Port:</p>
                                        <asp:TextBox id="sql_Port" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Databas:</p>
                                        <asp:TextBox id="sql_Database" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Användarnamn:</p>
                                        <asp:TextBox id="sql_Username" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">lösenord:</p>
                                        <asp:TextBox id="sql_Password" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <button type="button" class="btn form-control btn-primary next-step">Nästa</button>
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="step3">
                                    <div class="col-md-6 col-md-offset-3">
                                        <h3>Övriga Inställningar:</h3>
                                        <p style="font-size: 14px; margin-bottom: 2px;">Licens fil:</p>                         
                                        <asp:TextBox id="licensePath" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Domän:</p>
                                        <asp:TextBox id="Domain" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <p style="font-size: 14px; margin-bottom: 2px;">Gränsnit sökväg:</p>
                                        <asp:TextBox id="uiPath" class="form-control mr-sm-2" type="text" runat="server" />
                                        <br />
                                        <i>Vänligen kontrollera att alla uppgifter stämmer innan du trycker spara</i>
                                        <i>Detta kommer att ta någon minut</i>
                                        <asp:Button runat="server" OnClick="Save_Click" type="button" class="btn form-control btn-primary btn-info-full next-step" Text="Spara" />

                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="complete">
                                    <h3>Installationen är nu klar</h3>
                                    <p>Om det var något du missade kan du gå tillbaka.</p>
                                </div>
                                <div class="clearfix"></div>
                            </div>
                        </form>
                    </div>
                </section>
            </div>
        </div>
    </form>

    <script>
        $(document).ready(function () {
            //Initialize tooltips
            $('.nav-tabs > li a[title]').tooltip();

            //Wizard
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

                var $target = $(e.target);

                if ($target.parent().hasClass('disabled')) {
                    return false;
                }
            });

            $(".next-step").click(function (e) {

                var $active = $('.wizard .nav-tabs li.active');
                $active.next().removeClass('disabled');
                nextTab($active);

            });
            $(".prev-step").click(function (e) {

                var $active = $('.wizard .nav-tabs li.active');
                prevTab($active);

            });
        });

        function nextTab(elem) {
            $(elem).next().find('a[data-toggle="tab"]').click();
        }
        function prevTab(elem) {
            $(elem).prev().find('a[data-toggle="tab"]').click();
        }
    </script>
</body>
</html>
