<template>
    <div id="divimportgames" class="mx-auto" style="max-width:400px;">
        <div v-if="loading">
            <div class="d-flex m-3">
                <div class="mx-auto">
                    <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
                </div>
            </div>
        </div>
        <div v-else class="row g-2 justify-content-center mb-3">  
            <div class="d-flex mb-1 align-items-center">
                <font-awesome-icon icon="fa-solid fa-triangle-exclamation" size="lg" class="text-warning me-2"/>
                <div class="align-self-end text-xs text-muted">
                    <span>Your Profile must be <a href="#/" tabindex="0" data-bs-toggle="popover" data-bs-trigger="focus" data-bs-title="Steam Import" data-bs-content="Your <strong>Steam Profile</strong> and <strong>Game Details</strong> section under your <a href='https://help.steampowered.com/en/faqs/view/588C-C67D-0251-C276' target='_blank'>Steam Privacy Settings</a> must be Public in order to import Steam games. No other section needs to be Public.<br/><br/>If you have privacy concerns you can set <strong>Game Details</strong> to Public before importing and set it back to Private/Friends Only afterwards.">Public</a> to import Steam games</span>
                </div>
            </div>             
            <div class="btn-group mt-0" role="group">
                <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 1)" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]">
                    <font-awesome-icon icon="fa-brands fa-steam" size="xl"/>
                    <div class="mx-auto">
                        <span>{{ (importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id] ? 'Importing ' : 'Import ') + 'Steam games' }}</span>
                        <div v-if="importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.relativeImportLastRunDateString" class="text-xs">
                            <span>{{ 'Imported ' + importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.relativeImportLastRunDateString }}</span>
                        </div>
                    </div>
                    <font-awesome-icon v-if="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
                <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 30px;" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]">
                    <span class="visually-hidden">Toggle Dropdown</span>
                </button>                    
                <ul class="dropdown-menu">
                    <li><a :href="importGamesVM.steamAuthUrl" class="dropdown-item" data-value="0">Re-authenticate</a></li>
                </ul>                   
            </div>  
            <div class="btn-group" role="group">
                <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 2)" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]">
                    <font-awesome-icon icon="fa-brands fa-xbox" size="xl"/>
                    <div class="mx-auto">
                        <span>{{ (importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id] ? 'Importing ' : 'Import ') + 'Xbox games' }}</span>
                        <div v-if="importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.relativeImportLastRunDateString" class="text-xs">
                            <span>{{ 'Imported ' + importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.relativeImportLastRunDateString }}</span>
                        </div>
                    </div>
                    <font-awesome-icon v-if="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
                <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 30px;" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]">
                    <span class="visually-hidden">Toggle Dropdown</span>
                </button>                    
                <ul class="dropdown-menu">
                    <li><a :href="importGamesVM.microsoftAuthUrl" class="dropdown-item" data-value="0">Re-authenticate</a></li>
                </ul>                   
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
        },
        data() {
            return {
                importGamesVM: {
                    userAccounts: [],
                    steamAuthUrl: '',
                    microsoftAuthUrl: '',
                    authSuccess: null,
                    authAccountTypeID: null
                },
                importingUserAccountIDs: JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {},
                loading: false
            }
        },                                 
        watch: {
            importingUserAccountIDs: {
                handler(val, oldVal) {
                    sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(val));
                    var isImporting = Object.keys(val).length > 0;
                    this.$emit('isimportingupdate', isImporting);
                },
                deep: true
            }             
        },     
        mounted: function () {
            var that = this;
            
            window.addEventListener('importingUserAccountIDsUpdate', that.onImportingUserAccountIDsUpdate);
        },
        destroyed() {
            window.removeEventListener('importingUserAccountIDsUpdate', this.onImportingUserAccountIDsUpdate);
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
                            new Popover(el, { html: true });
                        });

                        if (that.importGamesVM.authSuccess != null) {
                            if (that.importGamesVM.authSuccess) {
                                that.importGames(that.importGamesVM.authAccountTypeID);
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

                return axios.get('/Game/ImportGames')
                    .then(res => {
                        that.importGamesVM = res.data;
                        that.loading = false;

                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onImportingUserAccountIDsUpdate: function() {
                var that = this;
                that.importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            },                           
            onImportGamesClick(e, accountTypeID) {                
                this.importGames(accountTypeID);
            },      
            importGames(accountTypeID) {
                var that = this;
                var userAccount = that.importGamesVM.userAccounts.find(i => i.accountTypeID == accountTypeID);
                
                if (userAccount) {
                    if (!that.importingUserAccountIDs.hasOwnProperty(userAccount.id)) {
                        that.importingUserAccountIDs[userAccount.id] = { userListName: userAccount.userListName };
                    }

                    axios.post('/Game/ImportGames', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                           params: { userAccountID: userAccount?.id } }).then((res) => {
                        if (res.data.isAuthExpired) {
                            var redirectUrl = that.getRedirectUrl(accountTypeID);
                            location.href = redirectUrl;
                        } else {
                            if (res.data.success) {
                                successToast("Imported <strong>" + res.data.count + "</strong> new games into <strong>" + userAccount.userListName + "</strong>");
                            } else {
                                res.data.errorMessages.forEach(errorMsg => {
                                    errorToast(errorMsg);                                         
                                });                                             
                            }
                        }                                                                                                                           
                        delete that.importingUserAccountIDs[userAccount.id];
                        that.loadData();
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
                } else {
                    var redirectUrl = that.getRedirectUrl(accountTypeID);
                    location.href = redirectUrl;                    
                }
            },
            getRedirectUrl(accountTypeID) {
                var result = '';

                switch (accountTypeID) {
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


