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
            <div class="d-flex mb-1">
                <font-awesome-icon icon="fa-solid fa-triangle-exclamation me-1" size="lg" class="text-warning me-2"/>
                <div class="align-self-end text-center text-xs text-muted">
                    <span>Your Steam Profile must be <a href="https://help.steampowered.com/en/faqs/view/588C-C67D-0251-C276">Public</a> to import Steam games</span>
                </div>
            </div>             
            <div class="btn-group me-1 useraccount-btn-group" role="group">
                <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 1)" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]">
                    <font-awesome-icon icon="fa-brands fa-steam" size="2xl"/>
                    <div class="mx-auto">
                        <div class="align-self-start fs-5">
                            <span class="mx-auto useraccount-btn-text">{{ (importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id] ? 'Importing ' : 'Import ') + 'Steam games' }}</span>
                        </div>
                        <div v-if="importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.relativeImportLastRunDateString" class="align-self-end">
                            <small>{{ 'Imported ' + importGamesVM.userAccounts.find(i => i.accountTypeID == 1).relativeImportLastRunDateString }}</small>
                        </div>
                    </div>
                    <font-awesome-icon v-if="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
                <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 50px;" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 1)?.id]">
                    <span class="visually-hidden">Toggle Dropdown</span>
                </button>                    
                <ul class="dropdown-menu">
                    <li><a :href="importGamesVM.steamAuthUrl" class="dropdown-item" data-value="0">Reauthenticate</a></li>
                </ul>                   
            </div>  
            <div class="btn-group me-1 useraccount-btn-group" role="group">           
                <button type="button" class="btn btn-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 2)" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]">
                    <font-awesome-icon icon="fa-brands fa-xbox" size="2xl"/>
                    <div class="mx-auto">
                        <div class="align-self-start fs-5">
                            <span class="mx-auto useraccount-btn-text">{{ (importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id] ? 'Importing ' : 'Import ') + 'Xbox games' }}</span>
                        </div>
                        <div v-if="importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.relativeImportLastRunDateString" class="align-self-end">
                            <small>{{ 'Imported ' + importGamesVM.userAccounts.find(i => i.accountTypeID == 2).relativeImportLastRunDateString }}</small>
                        </div>
                    </div>
                    <font-awesome-icon v-if="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
                <button type="button" class="btn btn-primary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false" style="max-width: 50px;" :disabled="importingUserAccountIDs[importGamesVM.userAccounts.find(i => i.accountTypeID == 2)?.id]">
                    <span class="visually-hidden">Toggle Dropdown</span>
                </button>                    
                <ul class="dropdown-menu">
                    <li><a :href="importGamesVM.microsoftAuthUrl" class="dropdown-item" data-value="0">Reauthenticate</a></li>
                </ul> 
            </div>
        </div>
        <div ref="steamimportwarnmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Import Games</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <span>Your Steam Profile and Game Details privacy settings must be set to public for the import to find any games</span>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal" @click="onImportGamesClick($event, 1)">Continue</button>
                    </div>                      
                </div>
            </div>
        </div>          
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    import { Tooltip } from 'bootstrap'; 
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        emits: ["isimportingupdate"],
        props: {
            isimportshown: Boolean,
            authsuccess: Boolean,
            authaccounttypeid: Number
        },
        data() {
            return {
                importGamesVM: {},
                importingUserAccountIDs: JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {},
                loading: false
            }
        },                                 
        watch: {
            isimportshown: function (val, oldVal) {
                if (val) {
                    this.loadData();
                }
            },
            importingUserAccountIDs: {
                handler(val, oldVal) {
                    sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(val));
                    var isImporting = Object.keys(val).length > 0;
                    this.$emit('isimportingupdate', isImporting);
                },
                deep: true
            }             
        },          
        created: function() {
            var that = this;
            that.loadData().then(i => {
                if (that.authsuccess != null) {
                    if (that.authsuccess) {
                        that.importGames(that.authaccounttypeid);
                    } else {            
                        that.errorToast("Error authorizing account");       
                    }
                }
            });
        },         
        mounted: function () {
            var that = this;

            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
                    new Tooltip(el);
                });

            window.addEventListener('importingUserAccountIDsUpdate', (event) => {
                that.importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            });
        },        
        methods: {       
            loadData: function () {
                var that = this;
                this.loading = true;

                return axios.get('/User/ImportGames')
                    .then(res => {
                        that.importGamesVM = res.data;
                        that.loading = false;

                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },               
            onImportGamesClick(e, accountTypeID) {
                this.importGames(accountTypeID)
            },            
            importGames(accountTypeID) {
                var that = this;
                var userAccount = that.importGamesVM.userAccounts.find(i => i.accountTypeID == accountTypeID);
                
                if (userAccount) {
                    if (!that.importingUserAccountIDs.hasOwnProperty(userAccount.id)) {
                        that.importingUserAccountIDs[userAccount.id] = { userListName: userAccount.userListName };
                    }

                    axios.post('/User/ImportGames', null,{ params: { userAccountID: userAccount?.id } }).then((res) => {
                        if (res.data.isAuthExpired) {
                            var redirectUrl = that.getRedirectUrl(accountTypeID);
                            location.href = redirectUrl;
                        } else {
                            if (res.data.success) {
                                successToast("Imported <strong>" + res.data.count + "</strong> new games into <strong>" + userAccount.userListName + "</strong>");
                            } else {
                                errorToast("Error importing into <strong>" + userAccount.userListName + "</strong>");                                               
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


