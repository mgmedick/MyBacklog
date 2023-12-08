<template>
    <div id="divimportgames">
        <div class="d-flex justify-content-center align-items-end mb-3">
            <div class="mx-auto">
                <span>Import games into <strong>{{ userlist?.name }}</strong>?</span>
            </div>
            <div class="align-self-end">
                <button type="button" class="btn btn-info p-2" tabindex="0" data-bs-toggle="popover" data-bs-trigger="focus" data-bs-title="Instructions" data-bs-content="">
                    <font-awesome-icon icon="fa-solid fa-circle-info" size="xl"/>
                    <div class="d-none popover-content">
                        <div>
                            <div class="mb-3">
                                <div><strong>CSV</strong></div>
                                <ul class="list-group list-group-flush no-gutters">
                                    <li class="list-group-item p-1">Download the template</li>
                                    <li class="list-group-item p-1">Fill in with your game names</li>
                                    <li class="list-group-item p-1">Upload spreadsheet</li>
                                </ul>
                            </div>
                            <div>
                                <div><strong>Steam & Xbox</strong></div>
                                <div class="mb-1">
                                    Login through the provider and your library will be imported.
                                </div>
                                <div class="alert alert-light">
                                    <span class='text-info me-1'>Note:</span>The <i>Profile</i> and <i>Game Details</i> sections under your <a href='https://help.steampowered.com/en/faqs/view/588C-C67D-0251-C276' target='_blank'>Steam Privacy Settings</a> must be Public to import Steam games.
                                </div>
                            </div>                            
                        </div>
                    </div>
                </button> 
            </div>
        </div>
        <div style="max-width:400px;" class="mx-auto">         
            <div v-if="loading">
                <div class="d-flex m-3">
                    <div class="mx-auto">
                        <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
                    </div>
                </div>
            </div>
            <div v-else>              
                <div class="row g-2 justify-content-center mb-3">
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesFromCSVClick" :disabled="importingTypeIDs.indexOf(1) > -1">
                            <font-awesome-icon icon="fa-solid fa-file-csv" size="xl"/>
                            <div class="mx-auto">
                                <span>CSV</span>
                            </div>
                            <font-awesome-icon v-if="importingTypeIDs.indexOf(1) > -1" icon="fa-solid fa-spinner" spin size="xl"/>
                        </button>
                        <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 30px;" :disabled="importingTypeIDs.indexOf(1) > -1">
                            <span class="visually-hidden">Toggle Dropdown</span>
                        </button>                    
                        <ul class="dropdown-menu">
                            <li><a @click="onDownloadTemplateClick" href="#/" class="dropdown-item" data-value="0">Download template</a></li>
                        </ul>                   
                    </div>              
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesFromUserAccountClick(2)" :disabled="importingTypeIDs.indexOf(2) > -1">
                            <font-awesome-icon icon="fa-brands fa-steam" size="xl"/>
                            <div class="mx-auto">
                                <span>Steam</span>
                            </div>
                            <font-awesome-icon v-if="importingTypeIDs.indexOf(2) > -1" icon="fa-solid fa-spinner" spin size="xl"/>
                        </button>
                        <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 30px;" :disabled="importingTypeIDs.indexOf(2) > -1">
                            <span class="visually-hidden">Toggle Dropdown</span>
                        </button>                    
                        <ul class="dropdown-menu">
                            <li><a :href="importGamesVM.steamAuthUrl" class="dropdown-item" data-value="0">Re-authenticate</a></li>
                        </ul>                   
                    </div>  
                    <div class="btn-group" role="group">
                        <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesFromUserAccountClick(3)" :disabled="importingTypeIDs.indexOf(3) > -1">
                            <font-awesome-icon icon="fa-brands fa-xbox" size="xl"/>
                            <div class="mx-auto">
                                <span>Xbox</span>
                            </div>
                            <font-awesome-icon v-if="importingTypeIDs.indexOf(3) > -1" icon="fa-solid fa-spinner" spin size="xl"/>
                        </button>
                        <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 30px;" :disabled="importingTypeIDs.indexOf(3) > -1">
                            <span class="visually-hidden">Toggle Dropdown</span>
                        </button>                    
                        <ul class="dropdown-menu">
                            <li><a :href="importGamesVM.microsoftAuthUrl" class="dropdown-item" data-value="0">Re-authenticate</a></li>
                        </ul>                   
                    </div>
                </div>   
            </div>       
        </div>
    </div> 
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    import { Tooltip, Popover } from 'bootstrap'; 
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        emits: ["isimportingupdate"],
        props: {
            userlist: Object
        },
        data() {
            return {
                importGamesVM: {
                    steamAuthUrl: '',
                    microsoftAuthUrl: '',
                    authSuccess: null,
                    authImportTypeID: null,
                    authAccountTypeID: null
                },
                importingTypeIDs: JSON.parse(sessionStorage.getItem('importingTypeIDs')) ?? [],         
                loading: false
            }
        },                                 
        watch: {
            importingTypeIDs: {
                handler(val, oldVal) {
                    sessionStorage.setItem('importingTypeIDs', JSON.stringify(val));
                    var isImporting = val.length > 0;
                    this.$emit('isimportingupdate', isImporting);
                },
                deep: true
            }                             
        },  
        mounted: function () {            
            window.addEventListener('importingTypeIDsUpdate', this.onImportingTypeIDsUpdate);
        },
        destroyed() {
            window.removeEventListener('importingTypeIDsUpdate', this.onImportingTypeIDsUpdate);
        },                          
        methods: {
            init: function() {
                var that = this;
                that.loadData().then(i => {
                    that.$nextTick(function() {
                        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
                            new Tooltip(el);
                        });

                        document.querySelectorAll('[data-bs-toggle="popover"]').forEach(el => {
                            new Popover(el, { 
                                html: true,
                                sanitize: false,
                                content: function() {
                                    return this.querySelector('.popover-content').innerHTML;
                                } 
                            });
                        });

                        if (that.importGamesVM.authSuccess != null) {
                            if (that.importGamesVM.authSuccess) {
                                that.onImportGamesFromUserAccountClick(that.importGamesVM.authImportTypeID);
                            } else {            
                                that.errorToast("Error authorizing account");       
                            }
                        }
                    });
                });
            },       
            loadData: function () {
                var that = this;
                this.loading = true;

                return axios.get('/UserList/ImportGames')
                    .then(res => {
                        that.importGamesVM = res.data;
                        that.loading = false;

                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },       
            onImportingTypeIDsUpdate: function() {
                this.importingTypeIDs = JSON.parse(sessionStorage.getItem('importingTypeIDs')) ?? [];
            },                  
            onImportGamesFromCSVClick(e, importTypeID, accountTypeID) {   
                var that = this;

                if (that.importingTypeIDs.indexOf(importTypeID) == -1) {
                    that.importingTypeIDs.push(importTypeID);
                }

                // that.importGamesFromAccount(importTypeID, accountTypeID);   
            },                                      
            onImportGamesFromUserAccountClick(importTypeID) {
                var that = this;

                if (that.importingTypeIDs.indexOf(importTypeID) == -1) {
                    that.importingTypeIDs.push(importTypeID);
                }

                return axios.post('/UserList/ImportGamesFromUserAccount', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                        params: { importTypeID: importTypeID, userListID: that.userlist.id } }).then((res) => {
                    if (res.data.isAuthExpired) {
                        var redirectUrl = that.getRedirectUrl(importTypeID);
                        location.href = redirectUrl;
                    } else {
                        if (res.data.success) {
                            successToast("Imported <strong>" + res.data.count + "</strong> games into <strong>" + that.userlist.name + "</strong>");
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                                         
                            });                                             
                        }

                        that.importingTypeIDs = that.importingTypeIDs.filter(i => i != importTypeID);
                        that.loadData(); 
                    }                                                                                                                           
                })
                .catch(err => { console.error(err); return Promise.reject(err); });
            },
            getRedirectUrl(importTypeID) {
                var result = '';

                switch (importTypeID) {
                    case 1:
                        result = this.importGamesVM.steamAuthUrl;
                        break;
                    case 2:
                        result = this.importGamesVM.microsoftAuthUrl;
                        break;                     
                }

                return result;
            }                                                                                                                             
        }
    };
</script>


