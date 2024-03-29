﻿<template>
    <div id="divimportgames">
        <div class="d-flex justify-content-center align-items-end mb-3">           
            <div class="mx-auto">
                <span style="margin-left: 40px;">Import games into <strong>{{ userlist?.name }}</strong>?</span>
            </div>
            <div>
                <a href="#/" role="button" class="btn btn-info p-2" tabindex="0" data-bs-toggle="popover" data-bs-trigger="focus" data-bs-title="Instructions" data-bs-content="">
                    <font-awesome-icon icon="fa-solid fa-circle-info" size="xl"/>
                    <div class="d-none popover-content">
                        <div>
                            <div class="mb-3">
                                <div><strong>CSV</strong></div>
                                <div class="mb-1">
                                    Upload a file (csv, xls, xlsx, txt) with your game names listed in the first column (new row per game).
                                </div>
                            </div>
                            <div>
                                <div><strong>Steam & Xbox</strong></div>
                                <div class="mb-1">
                                    Login through the provider and your library will be imported automatically.
                                </div>
                                <div class="alert alert-light">
                                    <span class='text-info me-1'>Note:</span>The <i>Profile</i> and <i>Game Details</i> sections under your <a href='https://help.steampowered.com/en/faqs/view/588C-C67D-0251-C276' target='_blank'>Steam Privacy Settings</a> must be Public to import Steam games.
                                </div>
                            </div>                            
                        </div>
                    </div>
                </a> 
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
                <!-- <div class="text-center">
                    <label for="formFile" class="form-label"><small class="fw-bold">Import from File</small></label>
                    <div class="btn-group" role="group">
                        <input id="formFile" type="file" v-model.lazy="form.Password" class="form-control form-control-sm" >
                        <button type="button" class="btn btn-primary btn-sm p-1" data-bs-toggle="dropdown" aria-expanded="false">
                            <font-awesome-icon icon="fa-solid fa-file-import" size="xl"/>
                        </button> 
                    </div>
                </div>           
                <div class="text-center"><small class="fw-bold">OR</small></div>                  -->
                <div class="row g-2 justify-content-center mb-3">
                    <button class="btn btn-primary d-flex justify-content-center align-items-center" :disabled="importingTypeIDs.indexOf(1) > -1">
                        <font-awesome-icon icon="fa-solid fa-file-import" size="xl"/>
                        <div class="mx-auto"><span style="margin-right:30px;">Import from <strong data-bs-toggle="tooltip" data-bs-title="csv, xls, xlsx, txt">File</strong></span></div><input ref="fileinput" type="file" @change="onFileChange" accept=".rtf, .txt, .csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" class="form-control form-control-sm" hidden>
                        <font-awesome-icon v-if="importingTypeIDs.indexOf(1) > -1" icon="fa-solid fa-spinner" spin size="xl"/>
                    </button>
                    <div class="btn-group p-0" role="group">
                        <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesFromUserAccountClick(2)" :disabled="importingTypeIDs.indexOf(2) > -1">
                            <font-awesome-icon icon="fa-brands fa-steam" size="xl"/>
                            <div class="mx-auto">
                                <span>Import from <strong>Steam</strong></span>
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
                    <div class="btn-group p-0" role="group">
                        <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesFromUserAccountClick(3)" :disabled="importingTypeIDs.indexOf(3) > -1">
                            <font-awesome-icon icon="fa-brands fa-xbox" size="xl"/>
                            <div class="mx-auto">
                                <span>Import from <strong>Xbox</strong></span>
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
    import { getFormData, successToast, errorToast } from '../../js/common.js';
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
                    if (val) {
                        sessionStorage.setItem('importingTypeIDs', JSON.stringify(val));
                    }
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
            onFileChange(e) {
                var that = this;
                var importTypeID = 1;
                
                if (this.$refs.fileinput.value) {
                    if (that.importingTypeIDs.indexOf(importTypeID) == -1) {
                        that.importingTypeIDs.push(importTypeID);
                    }

                    var form = { file: e.target.files[0], userListID: that.userlist.id };
                    var formData = getFormData(form);
                    var config = { headers: { 'RequestVerificationToken': that.getCsrfToken(), 'Content-Type': 'multipart/form-data' } };
                    return axios.post('/UserList/ImportGamesFromFile', formData, config)
                        .then((res) => {
                            if (res.data.success) {
                                successToast("Imported <strong>" + res.data.count + "</strong> games into <strong>" + that.userlist.name + "</strong>");
                            } else {
                                res.data.errorMessages.forEach(errorMsg => {
                                    errorToast(errorMsg);                           
                                });
                            }

                            that.$refs.fileinput.value = '';
                            that.importingTypeIDs = that.importingTypeIDs.filter(i => i != importTypeID);
                            that.loadData();                         
                        })
                        .catch(err => { 
                            console.error(err);
                            that.importingTypeIDs = that.importingTypeIDs.filter(i => i != importTypeID);

                            return Promise.reject(err);
                        });
                }
            },                          
            onImportingTypeIDsUpdate: function() {
                this.importingTypeIDs = JSON.parse(sessionStorage.getItem('importingTypeIDs')) ?? [];
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
                .catch(err => { 
                    console.error(err);
                    that.importingTypeIDs = that.importingTypeIDs.filter(i => i != importTypeID);

                    return Promise.reject(err); 
                });
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


